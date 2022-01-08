﻿using Rsbc.Dmf.CaseManagement.Service;
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
        public string Status { get; set; }
        public string Title { get; set; }

    }

    public class CaseService : ICaseQueryService
    {
        private readonly CaseManager.CaseManagerClient caseManager;
        private readonly IUserService userService;

        public CaseService(CaseManager.CaseManagerClient caseManager, IUserService userService)
        {
            this.caseManager = caseManager;
            this.userService = userService;
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
                if (! canAccess)
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
                searchRequest.ClinicId = userContext.CurrentClinicAssignment.ClinicId;
            }
            
            searchRequest.Statuses.Add(query.ByStatus);

            var results = (await caseManager.SearchAsync(searchRequest)).Items;

            return results.Select(c => new DmerCaseListItem
            {
                Id = c.CaseId,
                Title = c.Title,
                ClinicName = c.Provider?.Name,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn.ToDateTime(),
                ModifiedBy = c.ModifiedBy,
                ModifiedOn = c.ModifiedOn.ToDateTime(),
                PatientName = c.Driver.Name,
                DriverLicense = c.Driver.DriverLicenceNumber,
                Status = c.Status
            });
        }
    }
}