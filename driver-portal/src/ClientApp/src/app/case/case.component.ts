import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDetail } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, DmerTypeComponent, EligibleLicenseClassComponent, RecentCaseComponent, PortalsEnum, SharedQuickLinksComponent } from '@shared/core-ui';

@Component({
    selector: 'app-case',
    templateUrl: './case.component.html',
    styleUrls: ['./case.component.scss'],
    standalone: true,
    imports: [
        SharedQuickLinksComponent,
        RecentCaseComponent,
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
        DatePipe,
    ],
})
export class CaseComponent implements OnInit {
  isExpanded: Record<string, boolean> = {};
  isLoading = true;

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  _closedCaseDetails: CaseDetail[] | null = [];

  PortalsEnum = PortalsEnum;

  @Input() set closedCaseDetails(caseDetails: CaseDetail[] | null) {
    if (caseDetails !== undefined) {
      this._closedCaseDetails = caseDetails;

      this._closedCaseDetails?.forEach((cases) => {
        if (cases.caseId) this.isExpanded[cases.caseId] = false;
      });
    }
  }

  get closedCaseDetails() {
    return this._closedCaseDetails;
  }

  constructor(
    public caseManagementService: CaseManagementService,
    private loginService: LoginService
  ) {}

  ngOnInit(): void {
    if (this.loginService.userProfile?.id) {
      this.getClosedCases(this.loginService.userProfile?.id as string);
    } else {
      console.log('No user profile');
    }
  }

  getClosedCases(driverId: string) {
    this.caseManagementService
      .getClosedCases({ driverId })
      .subscribe((closedCases: any) => {
        this.closedCaseDetails = closedCases;
        this.isLoading = false;
      });
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }
}
