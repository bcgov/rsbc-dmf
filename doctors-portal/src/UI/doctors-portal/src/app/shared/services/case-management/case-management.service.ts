import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CaseManagementService {

  private cases = [
    { caseId: 'DMR1234', patientName: 'Rahul Minto', lastUpdatedOn: new Date('05/10/2021'), lastUpdatedBy: 'Sharon Torres' },
    { caseId: 'DMR9723', patientName: 'Randy Norman', lastUpdatedOn: new Date('04/27/2021'), lastUpdatedBy: 'Dr. Shelby Drew' },
    { caseId: 'DMR6430', patientName: 'Daniel Hoffman', lastUpdatedOn: new Date('04/24/2021'), lastUpdatedBy: 'Dr. Devi Iyer' },
    { caseId: 'DMR8245', patientName: 'Margy Klein', lastUpdatedOn: new Date('04/23/2021'), lastUpdatedBy: 'Dr. Tarik Haiga' },
  ];

  constructor() { }

  public getCases(): DMERForm[] {
    return [...this.cases];
  }
}

export interface DMERForm {
  caseId: string;
  patientName: string;
  lastUpdatedOn: Date;
  lastUpdatedBy: string;
}
