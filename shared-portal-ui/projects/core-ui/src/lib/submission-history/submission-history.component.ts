import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { Document } from '../api';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { CaseTypeComponent } from '../case-definitions/case-type/case-type.component';
import { SubmissionStatusComponent } from '../case-definitions/submission-status/submission-status.component';
import { SubmissionTypeComponent } from '../case-definitions/submission-type/submission-type.component';
import { PortalsEnum, SubmittalStatusEnum } from '../app.model';
import { SharedQuickLinksComponent } from "../quick-links/quick-links.component";

@Component({
    selector: 'app-shared-submission-history',
    templateUrl: './submission-history.component.html',
    styleUrls: ['./submission-history.component.scss'],
    standalone: true,
    imports: [
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
    SharedQuickLinksComponent
],
})
export class SharedSubmissionHistoryComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  isExpanded: Record<string, boolean> = {};

  @Input() caseManagementService: any;
  @Input()  portal!: PortalsEnum;

  PortalsEnum = PortalsEnum;
  
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

    this.filteredDocuments = this.submissionHistoryDocuments?.slice(0, pageSize);
  }
}
