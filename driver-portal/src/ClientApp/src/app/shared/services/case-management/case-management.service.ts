import { Injectable } from '@angular/core';
import { CasesService } from '../../api/services';

@Injectable({
  providedIn: 'root'
})
export class CaseManagementService {

  constructor(private casesService: CasesService) { }

  public getCaseById(params: Parameters<CasesService['apiCasesCaseIdGet$Json']>[0]) {
    return this.casesService.apiCasesCaseIdGet$Json(params);
  }
}
