import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-submission-history',
  templateUrl: './submission-history.component.html',
  styleUrls: ['./submission-history.component.css'],
})
export class SubmissionHistoryComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
  isExpanded: Record<string, boolean> = {};

  _allDocuments?: Document[] | null = [];

  @Input() set allDriverDocuments(documents: Document[] | null | undefined) {
    this._allDocuments = documents;

    this._allDocuments?.forEach((doc) => {
      if (doc.documentId) this.isExpanded[doc.documentId] = false;
    });
  }

  get allDriverDocuments() {
    return this._allDocuments;
  }

  constructor(private caseManagementService: CaseManagementService) {}

  ngOnInit(): void {
    this.getAllDocuments('e27d7c69-3913-4116-a360-f5e990200173');
  }

  getAllDocuments(driverId: string) {
    this.caseManagementService
      .getAllDocuments({ driverId })
      .subscribe((allDocuments: any) => {
        this._allDocuments = allDocuments;
        console.log(allDocuments);
      });
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }
}
