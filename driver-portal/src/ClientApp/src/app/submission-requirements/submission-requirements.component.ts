import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.css'],
})
export class SubmissionRequirementsComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  public driverDocuments: Document[] = [];

  constructor(private caseManagementService: CaseManagementService) {}

  ngOnInit(): void {
    this.getSubmissionRequireDocuments('e27d7c69-3913-4116-a360-f5e990200173');
  }

  getSubmissionRequireDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((documents) => {
        if (
          documents?.caseSubmissions &&
          documents?.caseSubmissions?.length > 0
        ) {
          this.driverDocuments = documents.caseSubmissions;
        }

        console.log(documents);
      });
  }
}
