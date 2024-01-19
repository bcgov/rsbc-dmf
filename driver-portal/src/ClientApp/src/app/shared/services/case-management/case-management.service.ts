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
    params: Parameters<CasesService['apiCasesDriverIdClosedGet$Json']>[0]
  ) {
    return this.casesService.apiCasesDriverIdClosedGet$Json(params);
  }

  public getDriverDocuments(
    params: Parameters<DriverService['apiDriverDriverIdDocumentsGet$Json']>[0]
  ) {
    return this.driversService.apiDriverDriverIdDocumentsGet$Json(params);
  }

  public getAllDocuments(
    params: Parameters<
      DriverService['apiDriverDriverIdAllDocumentsGet$Json']
    >[0]
  ) {
    return this.driversService.apiDriverDriverIdAllDocumentsGet$Json(params);
  }
}
