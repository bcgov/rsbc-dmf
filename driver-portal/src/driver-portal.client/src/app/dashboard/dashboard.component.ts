import { Component, ViewChild } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router } from '@angular/router';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { ViewportScroller } from '@angular/common';
import { LetterTopicComponent } from '../case-definations/letter-topic/letter-topic.component';
import { SubmissionStatusComponent } from '../case-definations/submission-status/submission-status.component';
import { SubmissionTypeComponent } from '../case-definations/submission-type/submission-type.component';
import { EligibleLicenseClassComponent } from '../case-definations/eligible-license-class/eligible-license-class.component';
import { DecisionOutcomeComponent } from '../case-definations/decision-outcome/decision-outcome.component';
import { DmerTypeComponent } from '../case-definations/dmer-type/dmer-type.component';
import { CaseStatusComponent } from '../case-definations/case-status/case-status.component';
import { CaseTypeComponent } from '../case-definations/case-type/case-type.component';
import { RecentCaseComponent } from '../recent-case/recent-case.component';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: true,
    imports: [
        RecentCaseComponent,
        MatAccordion,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        CaseTypeComponent,
        CaseStatusComponent,
        DmerTypeComponent,
        DecisionOutcomeComponent,
        EligibleLicenseClassComponent,
        SubmissionTypeComponent,
        SubmissionStatusComponent,
        LetterTopicComponent,
    ],
})
export class DashboardComponent {
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router,
    private viewportScroller: ViewportScroller
  ) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
