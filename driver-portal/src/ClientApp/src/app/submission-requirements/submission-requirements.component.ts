import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.css'],
})
export class SubmissionRequirementsComponent {
  @Input() submissionRequirementDocuments?: Document[] | null = [];

  @Input() selectedIndex = 0;

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  navigateToLetters() {
    this.selectedIndex = this.selectedIndex + 2;
    console.log(this.selectedIndex);
  }
}
