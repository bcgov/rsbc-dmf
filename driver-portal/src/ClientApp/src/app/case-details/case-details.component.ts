import { Component, OnInit } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDocuments } from '../shared/api/models';

@Component({
  selector: 'app-case-details',
  templateUrl: './case-details.component.html',
  styleUrls: ['./case-details.component.css'],
})
export class CaseDetailsComponent implements OnInit {
  caseDocuments!: CaseDocuments;

  

  // public caseDocuments: Document[] = [];

  constructor(private caseManagementService: CaseManagementService) {}

  ngOnInit(): void {
    this.getCaseSubmissionDocuments('e27d7c69-3913-4116-a360-f5e990200173');
  }

  getCaseSubmissionDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((caseDocuments: any) => {
        this.caseDocuments = caseDocuments;
        console.log(caseDocuments);
      });
  }
}
