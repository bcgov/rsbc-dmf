import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { CaseManagementService, DMERCase, DMERSearchCases } from '../services/case-management/case-management.service';

@Injectable({
  providedIn: 'root'
})
export class CaseManagementStubService extends CaseManagementService {

  private cases: DMERCase[] = [
    { id: '111', patientName: 'p1', status: 'Pending', driverLicense: '1111' },
    { id: '222', patientName: 'p2', status: 'Completed', driverLicense: '2222' },
    { id: '333', patientName: 'p3', status: 'Active', driverLicense: '3333' },
  ];
  public getCases(params: DMERSearchCases): Observable<DMERCase[]> {
    return of(this.cases);
  }

}
