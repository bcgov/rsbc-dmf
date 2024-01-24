import { Injectable } from '@angular/core';
import { CasesService, DriverService } from '../../api/services';

@Injectable({
  providedIn: 'root',
})
export class CaseManagementService {
  constructor(
    private casesService: CasesService,
    private driversService: DriverService
  ) {}

  public getCaseById(
    params: Parameters<CasesService['apiCasesCaseIdGet$Json']>[0]
  ) {
    return this.casesService.apiCasesCaseIdGet$Json(params);
  }

  public getMostRecentCase(
    params: Parameters<CasesService['apiCasesMostRecentGet$Json']>[0]
  ) {
    return this.casesService.apiCasesMostRecentGet$Json(params);
  }

  public getClosedCases(
    params: Parameters<CasesService['apiCasesClosedGet$Json']>[0]
  ) {
    return this.casesService.apiCasesClosedGet$Json(params);
  }

  public getDriverDocuments(
    params: Parameters<DriverService['apiDriverDocumentsGet$Json']>[0]
  ) {
    return this.driversService.apiDriverDocumentsGet$Json(params);
  }

  public getAllDocuments(
    params: Parameters<
      DriverService['apiDriverAllDocumentsGet$Json']
    >[0]
  ) {
    return this.driversService.apiDriverAllDocumentsGet$Json(params);
  }
}
