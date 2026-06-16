using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pssg.DocumentStorageAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Models;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;

namespace Rsbc.Dmf.IcbcAdapter
{
	public class DmerNotifications
	{
		private readonly IConfiguration _configuration;
		private readonly CaseManager.CaseManagerClient _caseManagerClient;
		private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
		private readonly IIcbcClient _icbcClient;
		private readonly string _processedFolder;
        private readonly string _dmerFolder;

        // Fixed-width mapping based on DMETEXT record layout.
        private const int DmeRecordLength = 118;


		public DmerNotifications(
			IConfiguration configuration,
			CaseManager.CaseManagerClient caseManagerClient,
			IIcbcClient icbcClient,
			DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
		{
			_configuration = configuration;
			_caseManagerClient = caseManagerClient;
			_icbcClient = icbcClient;
			_documentStorageAdapterClient = documentStorageAdapterClient;
			_processedFolder = _configuration["DMER_PROCESSED_FOLDER"] ?? "dmer/proccessed";
			_dmerFolder = _configuration["DMER_FOlDER"]?? "dmer";
		}

		public async Task GetIcbcNotificationsAndUpdateCase()
		{
			var notifactions = await GetIcbcNotifications();
			if (notifactions?.NotificationFiles?.Count > 0)
			{
				foreach (var notification in notifactions.NotificationFiles.Values)
				{
					var notifications = await ParseIcbcNotication(notification);
					await CreateOrUpdateCases(notifications);
				}

				await MoveProcessedFilesToProcessedFolder(notifactions);
			}
		}

		internal async Task CreateOrUpdateCases(List<DmerNotificationRecord> notifications)
		{
			var dmerCases = notifications.Select(MapToDmerCase).ToList();

			await CreateOrUpdateCases(dmerCases, dmerCase => new CreateDmerCaseRequest
			{
				DriverLicenseNumber = dmerCase.DriverLicenseNumber,
				CaseTypeCode = dmerCase.CaseTypeCode,
				TriggerType = "DMER",
				Owner = dmerCase.Owner,
				DriverDateOfBirth = Timestamp.FromDateTime(
				dmerCase.DriverDateOfBirthUtc),
				DriverSurname = dmerCase.DriverSurname,
				ProgramArea = "DMF",
				DocumentOwner = "Team - Intake",
				DocumentType = "DMER"
            });
		}

		private DmerCaseRecord MapToDmerCase(DmerNotificationRecord source)
		{
			return new DmerCaseRecord
			{
				DriverLicenseNumber = source.Lnum,
				CaseTypeCode = "DMER",
                Owner = "Remedial",
                TriggerType = source.MedicalType,
				DriverDateOfBirthUtc = DateTime.SpecifyKind(DateTime.Parse(source.BirthDate), DateTimeKind.Utc),
				DriverSurname = source.Surname
			};
		}

		internal async Task CreateOrUpdateCases(List<DmerCaseRecord> cases, Func<DmerCaseRecord, CreateDmerCaseRequest> caseMapper)
		{
			try
			{
				var total = 0;
				foreach (var item in cases)
				{
					var caseToCreate = caseMapper(item);

					await _caseManagerClient.CreateDmerCaseAsync(caseToCreate);
					total++;
				}

				Log.Logger.Information($"Successfully proccessed {total} DMER cases see cms logs for more details");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Error creating/updating DMER cases: " + ex.Message);
			}
		}

		public async Task RemoveFilesFromIcbcS3Bucket(IEnumerable<string> ServerRelativeUrl)
		{
			if (_documentStorageAdapterClient == null)
			{
				throw new InvalidOperationException("Document storage adapter client is not configured.");
			}

			Log.Logger.Information("Removing DMER files from icbc S3 bucket");
			var request = new DeleteFilesInFolderRequest { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET" };
			request.ServerRelativeUrl.AddRange(ServerRelativeUrl);
			var result = await _documentStorageAdapterClient.DeleteFilesInFolderAsync(request);
			if (result.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
			{
				Log.Logger.Information("Successfully removed DMER files from icbc S3 bucket");
			}
		}

		private async Task MoveProcessedFilesToProcessedFolder(IcbcNotificationsFileResult notifactions)
		{
			if (_documentStorageAdapterClient == null)
			{
				throw new InvalidOperationException("Document storage adapter client is not configured.");
			}

			var fileNames = notifactions.NotificationFiles?.Keys.ToList() ?? new List<string>();
			var movedCount = 0;


			foreach (var fileName in fileNames)
			{
				try
				{
					await _documentStorageAdapterClient.MoveFileAsync(new MoveFileRequest() { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET", SourcePath = _dmerFolder, Destinationpath = _processedFolder, FileName = fileName });

				}
				catch (Exception ex)
				{
					Log.Logger.Error($"Unable to move DMER file {_dmerFolder} to {_processedFolder}: {ex.Message}");
				}
			}

			if (movedCount > 0)
			{
				Log.Logger.Information($"Moved {movedCount} DMER files to {_processedFolder}");
			}
		}

		private string BuildProcessedServerRelativeUrl(string sourceUrl)
		{
			var fileName = sourceUrl?
				.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
				.LastOrDefault();

			if (string.IsNullOrWhiteSpace(fileName))
			{
				fileName = $"dmer-{Guid.NewGuid():N}.dat";
			}

			return $"{_processedFolder}/{fileName}";
		}

		public async Task<List<DmerNotificationRecord>> ParseIcbcNotication(IFormFile file)
		{
			Log.Logger.Information("Parsing DMER notification dat file...");
			if (file == null || file.Length == 0)
			{
				throw new ArgumentException("File is empty or null.");
			}

			var records = new List<DmerNotificationRecord>();

			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				string line;
				while ((line = await reader.ReadLineAsync()) != null)
				{
					if (string.IsNullOrWhiteSpace(line))
					{
						continue;
					}

					var normalized = line.Length < DmeRecordLength
						? line.PadRight(DmeRecordLength)
						: line;

					var record = new DmerNotificationRecord
					{
						Lnum = Slice(normalized, 0, 8),
						Clno = Slice(normalized, 8, 9),
						Surname = Slice(normalized, 17, 35),
						MedicalIssueDate = Slice(normalized, 52, 10),
						MedicalType = Slice(normalized, 62, 1),
						Sex = Slice(normalized, 63, 1),
						BirthDate = Slice(normalized, 64, 10),
						LicenceExpiryDate = Slice(normalized, 74, 10),
						LastMedicalDate = Slice(normalized, 84, 10),
						LastExamDate = Slice(normalized, 94, 10),
						AddressDocumentDate = Slice(normalized, 104, 10),
						MasterStatusCode = Slice(normalized, 114, 1),
						LicenceClass = Slice(normalized, 115, 3)
					};

					var validationErrors = ValidateRecord(record);
					if (!string.IsNullOrEmpty(validationErrors))
					{
						Log.Logger.Warning("DMER record was not added: " + record + "\n Invalid values: " + validationErrors);
					}
					else
					{
						records.Add(record);
					}
				}
			}

			return records;
		}

		private static string Slice(string value, int start, int length)
		{
			if (string.IsNullOrEmpty(value) || start >= value.Length)
			{
				return string.Empty;
			}

			var size = Math.Min(length, value.Length - start);
			return value.Substring(start, size).Trim();
		}

		private string ValidateRecord(DmerNotificationRecord record)
		{
			var errors = string.Empty;

			if (string.IsNullOrWhiteSpace(record.Lnum) || record.Lnum.Contains(" "))
			{
				errors += "\nLNUM: " + record.Lnum;
			}

			if (string.IsNullOrWhiteSpace(record.MedicalType))
			{
				errors += "\nMDTP: " + record.MedicalType;
			}

			return string.IsNullOrEmpty(errors) ? null : errors;
		}

		private async Task<IcbcNotificationsFileResult> GetIcbcNotifications()
		{
			if (_documentStorageAdapterClient == null)
			{
				throw new InvalidOperationException("Document storage adapter client is not configured.");
			}

			var result = new IcbcNotificationsFileResult
			{
				NotificationFiles = new Dictionary<string, IFormFile>()
			};

			var files = await _documentStorageAdapterClient.DownloadFolderAsync(
				new DownloadFolderRequest { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET", FolderName = _dmerFolder });

			var fileNames = files.Files.Select(f => f.FileName).ToList();
			Log.Logger.Information("Fetching DMER notification dat file(s):" + string.Join(",", fileNames));

			if (files.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
			{
				foreach (var fileBytes in files.Files.Where(x=> x.Data.Length > 1))
				{
					var stream = new MemoryStream(fileBytes.Data.ToByteArray());
					var fileName = fileBytes.ServerRelativeUrl.Split('/').Last();

                    result.NotificationFiles[fileName] = new FormFile(stream, 0, stream.Length, "file", "DMER_Notifications")
					{
						Headers = new HeaderDictionary(),
						ContentType = "application/octet-stream"
					};
				}

				Log.Logger.Information($"Successfully fetched {result.NotificationFiles.Count} DMER files from icbc S3 bucket");
				return result;
			}

			return null;
		}
	}
}
