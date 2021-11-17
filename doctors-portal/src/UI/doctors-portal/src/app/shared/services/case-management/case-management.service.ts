import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { DmerCaseListItem } from '../../api/models';
import { CasesService } from '../../api/services';

@Injectable({
  providedIn: 'root'
})
export class CaseManagementService {

  constructor(private casesService: CasesService) { }

  public getCases(params: DMERSearchCases): Observable<DMERCase[]> {
    let searchParams = {
      ByTitle: params.byTitle,
      ByDriverLicense: params.byDriverLicense,
      ByPatientName: params.byPatientName,
      ByStatus: params.byStatus,
      ByClinicId: params.byClinicId
    };
    console.debug(searchParams);
    return this.casesService.apiCasesGet$Json(searchParams).pipe(map(cases => cases.map(c => c)));
  }
}

export interface DMERCase extends DmerCaseListItem { }

export interface DMERSearchCases {
  byTitle?: string,
  byDriverLicense?: string,
  byPatientName?: string,
  byClinicId?: string,
  byStatus?: string[]
}
