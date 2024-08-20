// IMPORTANT keep this file identical to driver-portal submission-history.component

import { Component, Input, OnInit, ViewChild } from '@angular/core';
import {
  MatAccordion,
  MatExpansionPanel,
  MatExpansionPanelHeader,
  MatExpansionPanelTitle,
} from '@angular/material/expansion';
import { Document } from '../shared/api/models';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { CaseTypeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-type/case-type.component';
import { SubmissionTypeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/submission-type/submission-type.component';
import { SubmissionStatusComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/submission-status/submission-status.component';
import { SubmittalStatusEnum } from '@app/app.model';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';

@Component({
  selector: 'app-submission-history',
  templateUrl: './submission-history.component.html',
  styleUrls: ['./submission-history.component.scss'],
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
    SubmissionTypeComponent,
    SubmissionStatusComponent,
    DatePipe,
  ],
})
export class SubmissionHistoryComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  isExpanded: Record<string, boolean> = {};

  pageSize = 10;

  isLoading = true;

  filteredDocuments?: Document[] | null = [];

  _allDocuments?: Document[] | null = [];

  @Input() set allDriverDocuments(documents: Document[] | null | undefined) {
    this._allDocuments = documents;

    this._allDocuments?.forEach((doc) => {
      if (doc.documentId) this.isExpanded[doc.documentId] = false;
    });

    this.filteredDocuments = this._allDocuments?.slice(0, this.pageSize);
  }

  get allDriverDocuments() {
    return this._allDocuments;
  }

  submissionHistoryDocuments: Document[] = [];

  constructor(
    private caseManagementService: CaseManagementService,
  ) {}

  ngOnInit(): void {
    this.getAllDocuments();
  }

  getAllDocuments() {
    this.caseManagementService
      .getAllDriverDocuments()
      .subscribe((allDocuments: any) => {
        if (!allDocuments) {
          return;
        }

        this._allDocuments = allDocuments;
        this.submissionHistoryDocuments = [];

        // can be moved to driver portal
        allDocuments.forEach((doc: any) => {
          if (
            ![
              SubmittalStatusEnum.OpenRequired,
              SubmittalStatusEnum.Sent,
              SubmittalStatusEnum.Issued,
            ].includes(doc.submittalStatus as SubmittalStatusEnum)
          ) {
            this.submissionHistoryDocuments.push(doc);
          }
        });

        this.filteredDocuments = this.submissionHistoryDocuments.slice(
          0,
          this.pageSize,
        );
        this.isLoading = false;
      });
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  viewMore() {
    const pageSize = (this.filteredDocuments?.length ?? 0) + this.pageSize;

    this.filteredDocuments = this.submissionHistoryDocuments?.slice(
      0,
      pageSize,
    );
  }
}
