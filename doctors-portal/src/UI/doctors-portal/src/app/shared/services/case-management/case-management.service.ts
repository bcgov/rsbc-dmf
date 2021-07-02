import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Case } from '../../api/models';
import { CasesService } from '../../api/services';

@Injectable({
  providedIn: 'root'
})
export class CaseManagementService {

  constructor(private casesService: CasesService) { }

  public getCases(params: DMERSearchCases): Observable<DMERCase[]> {
    let searchParams = {
      ByCaseId: params.byCaseId,
      ByDriverLicense: params.byDriverLicense,
      ByPatientName: params.byPatientName,
      ByStatus: params.byStatus
    };
    console.debug(searchParams);
    return this.casesService.apiCasesGet$Json(searchParams).pipe(map(cases => cases.map(c => c)));
  }
}

export interface DMERCase extends Case { }

export interface DMERSearchCases {
  byCaseId?: string,
  byDriverLicense?: string,
  byPatientName?: string,
  byStatus?: string[]
}
