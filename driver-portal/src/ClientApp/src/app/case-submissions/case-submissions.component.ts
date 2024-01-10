import { ViewportScroller } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-case-submissions',
  templateUrl: './case-submissions.component.html',
  styleUrls: ['./case-submissions.component.css'],
})
export class CaseSubmissionsComponent implements OnInit {
  isExpanded: Record<string, boolean> = {
    '1': false,
  };
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  toggleisExpandable(id: string) {
    this.isExpanded[id] = !this.isExpanded[id];
  }

  public submissionDocuments: Document[] = [];

  constructor(private caseManagementService: CaseManagementService) {}

  ngOnInit(): void {
    this.getCaseSubmissionDocuments('e27d7c69-3913-4116-a360-f5e990200173');
  }

  getCaseSubmissionDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((caseDocuments: any) => {
        if (
          caseDocuments?.caseSubmissions &&
          caseDocuments?.caseSubmissions?.length > 0
        ) {
          this.submissionDocuments = caseDocuments.caseSubmissions;
        }

        console.log(caseDocuments);
      });
  }
}
