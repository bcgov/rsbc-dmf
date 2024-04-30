import { Component, Input, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';
import { LetterTopicComponent } from '../case-definations/letter-topic/letter-topic.component';
import { SubmissionStatusComponent } from '../case-definations/submission-status/submission-status.component';
import { SubmissionTypeComponent } from '../case-definations/submission-type/submission-type.component';
import { RsbcCaseAssignmentComponent } from '../case-definations/rsbc-case-assignment/rsbc-case-assignment.component';
import { EligibleLicenseClassComponent } from '../case-definations/eligible-license-class/eligible-license-class.component';
import { DecisionOutcomeComponent } from '../case-definations/decision-outcome/decision-outcome.component';
import { DmerTypeComponent } from '../case-definations/dmer-type/dmer-type.component';
import { CaseStatusComponent } from '../case-definations/case-status/case-status.component';
import { CaseTypeComponent } from '../case-definations/case-type/case-type.component';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { QuickLinksComponent } from '../quick-links/quick-links.component';

@Component({
    selector: 'app-case-submissions',
    templateUrl: './case-submissions.component.html',
    styleUrls: ['./case-submissions.component.scss'],
    standalone: true,
    imports: [
        QuickLinksComponent,
        NgFor,
        MatCard,
        NgClass,
        MatCardContent,
        MatIcon,
        NgIf,
        MatAccordion,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        CaseTypeComponent,
        CaseStatusComponent,
        DmerTypeComponent,
        DecisionOutcomeComponent,
        EligibleLicenseClassComponent,
        RsbcCaseAssignmentComponent,
        SubmissionTypeComponent,
        SubmissionStatusComponent,
        LetterTopicComponent,
        DatePipe,
    ],
})
export class CaseSubmissionsComponent {
  _caseSubmissionDocuments?: Document[] | null = [];
  @Input() set caseSubmissionDocuments(
    documents: Document[] | null | undefined
  ) {
    this._caseSubmissionDocuments = documents;

    this.caseSubmissionDocuments?.forEach((doc) => {
      if (doc.documentId) this.isExpanded[doc.documentId] = false;
    });
  }

  get caseSubmissionDocuments() {
    return this._caseSubmissionDocuments;
  }

  isExpanded: Record<string, boolean> = {};
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  constructor(private caseManagementService: CaseManagementService) {}
}
