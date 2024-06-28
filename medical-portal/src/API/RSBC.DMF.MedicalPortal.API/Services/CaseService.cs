using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSBC.DMF.MedicalPortal.API.Services
{
    public interface ICaseQueryService
    {
        Task<IEnumerable<DmerCaseListItem>> SearchCases(CaseSearchQuery query);
    }

    public class CaseSearchQuery
    {
        public string ByTitle { get; set; }
        public string ByDriverLicense { get; set; }
        public string ByClinicId { get; set; }
        public IEnumerable<string> ByStatus { get; set; } = Array.Empty<string>();
    }

    public class MedicalCondition
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string FormId { get; set; }
    }

    public class DmerCaseListItem
    {
        public string ClinicName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DriverLicense { get; set; }
        public string Id { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string PatientName { get; set; }

        public string PatientFirstname { get; set; }

        public string PatientLastname { get; set; }

        public string PatientMiddlename { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string DmerType { get; set; }
        public DateTimeOffset? DriverBirthDate { get; set; }
        public bool IsStarted { get; set; }
        public IEnumerable<MedicalCondition> MedicalConditions { get; set; }
    }

    // TODO remove CaseService and use CaseManagerClient directly
    public class CaseService : ICaseQueryService
    {
        private readonly CaseManager.CaseManagerClient caseManager;
        private readonly IUserService userService;

        public CaseService(CaseManager.CaseManagerClient caseManager, IUserService userService)
        {
            this.caseManager = caseManager;
            this.userService = userService;
        }

        private bool GetDmerStarted(string caseId)
        {
            return true;
        }

        private IEnumerable<MedicalCondition> MapMedicalConditions(
            IEnumerable<MedicalConditionItem> medicalConditionItems)
        {
            var medicalConditions = new List<MedicalCondition>();
            if (medicalConditionItems != null)
            {
                foreach (var item in medicalConditionItems)
                {
                    var newMedicalCondition = new MedicalCondition()
                    {
                        Id = item.Identifier,
                        Description = item.Question,
                        FormId = item.FormId
                    };
                    medicalConditions.Add(newMedicalCondition);
                }
            }

            return medicalConditions;
        }

        public async Task<IEnumerable<DmerCaseListItem>> SearchCases(CaseSearchQuery query)
        {
            var userContext = await userService.GetCurrentUserContext();

            var searchRequest = new SearchRequest
            {
                Title = query.ByTitle ?? string.Empty,
                DriverLicenseNumber = query.ByDriverLicense ?? string.Empty,
            };

            if (!string.IsNullOrEmpty(query.ByClinicId))
            {
                // check that the user is a member of the clinic.
                var canAccess = userContext.ClinicAssignments.Where(x => x.ClinicId == query.ByClinicId).Any();
                if (!canAccess)
                {
                    return Array.Empty<DmerCaseListItem>();
                }
                else
                {
                    searchRequest.ClinicId = query.ByClinicId;
                }
            }
            else
            {
                searchRequest.ClinicId = userContext.CurrentClinicAssignment?.ClinicId;
            }

            searchRequest.Statuses.Add(query.ByStatus);

            var results = (await caseManager.SearchAsync(searchRequest)).Items;

            return results.Select(c => new DmerCaseListItem
            {
                Id = c.CaseId,
                Title = c.Title,
                ClinicName = c.ClinicName,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn.ToDateTime(),
                ModifiedBy = c.ModifiedBy,
                ModifiedOn = c.ModifiedOn.ToDateTime(),
                PatientName = c.Driver?.Name,
                PatientFirstname = c.Driver?.GivenName,
                PatientLastname = c.Driver?.Surname,
                PatientMiddlename = c.Driver?.Middlename,
                DriverLicense = c.Driver?.DriverLicenseNumber,
                Status = c.Status,
                DmerType = c.DmerType,
                DriverBirthDate = c.Driver?.BirthDate != null ? c.Driver?.BirthDate.ToDateTimeOffset() : null,
                MedicalConditions = MapMedicalConditions(c.MedicalConditions),
                IsStarted = GetDmerStarted(c.CaseId)
            });
        }
    }
}