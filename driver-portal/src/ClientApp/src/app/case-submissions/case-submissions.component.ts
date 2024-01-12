import { ViewportScroller } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-case-submissions',
  templateUrl: './case-submissions.component.html',
  styleUrls: ['./case-submissions.component.css'],
})
export class CaseSubmissionsComponent {
  @Input() caseSubmissionDocuments?: Document[] | null = [];

  isExpanded: Record<string, boolean> = {
    '1': false,
  };
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  toggleisExpandable(id: string) {
    this.isExpanded[id] = !this.isExpanded[id];
  }

  constructor(private caseManagementService: CaseManagementService) {}
}
