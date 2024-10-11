import { Component, ViewChild } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router } from '@angular/router';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { ViewportScroller } from '@angular/common';
import { CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, DmerTypeComponent, EligibleLicenseClassComponent, LetterTopicComponent, SubmissionStatusComponent, SubmissionTypeComponent, RecentCaseComponent, PortalsEnum } from '@shared/core-ui';

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
  PortalsEnum = PortalsEnum;

  constructor(
    public caseManagementService: CaseManagementService,
    private router: Router,
    private viewportScroller: ViewportScroller
  ) {}

  public onClick(event: any, elementId: string): void {
    event.preventDefault();
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
