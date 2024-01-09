import { Injectable } from '@angular/core';
import { CasesService, DriversService } from '../../api/services';

@Injectable({
  providedIn: 'root',
})
export class CaseManagementService {
  constructor(
    private casesService: CasesService,
    private driversService: DriversService
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

  public getDriverDocuments(
    params: Parameters<DriversService['apiDriversDriverIdDocumentsGet$Json']>[0]
  ) {
    return this.driversService.apiDriversDriverIdDocumentsGet$Json(params);
  }
}
