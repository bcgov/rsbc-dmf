import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';

@Component({
  selector: 'app-submission-history',
  templateUrl: './submission-history.component.html',
  styleUrls: ['./submission-history.component.scss'],
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

  constructor(
    private caseManagementService: CaseManagementService,
    private loginService: LoginService
  ) {}

  ngOnInit(): void {
    // this.getAllDocuments('e27d7c69-3913-4116-a360-f5e990200173');
    if (this.loginService.userProfile?.id) {
      this.getAllDocuments(this.loginService.userProfile?.id as string);
    } else {
      console.log('No Letter Documents');
    }
  }

  getAllDocuments(driverId: string) {
    this.caseManagementService
      .getAllDocuments({ driverId })
      .subscribe((allDocuments: any) => {
        this._allDocuments = allDocuments;
        this.filteredDocuments = this._allDocuments?.slice(0, this.pageSize);
        this.isLoading = false;
      });
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  viewMore() {
    const pageSize = (this.filteredDocuments?.length ?? 0) + this.pageSize;

    this.filteredDocuments = this._allDocuments?.slice(0, pageSize);
  }
}
