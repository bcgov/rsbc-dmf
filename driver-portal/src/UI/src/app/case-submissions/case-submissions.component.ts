import { Component, Input, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, DmerTypeComponent, EligibleLicenseClassComponent, LetterTopicComponent, RsbcCaseAssignmentComponent, SubmissionStatusComponent, SubmissionTypeComponent } from '@shared/core-ui';

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
