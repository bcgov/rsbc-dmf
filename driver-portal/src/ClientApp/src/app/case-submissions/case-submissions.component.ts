import { Component, Input, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-case-submissions',
  templateUrl: './case-submissions.component.html',
  styleUrls: ['./case-submissions.component.scss'],
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
