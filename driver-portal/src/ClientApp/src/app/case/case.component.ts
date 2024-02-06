import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { CaseDetail } from '../shared/api/models';
import { LoginService } from '../shared/services/login.service';

@Component({
  selector: 'app-case',
  templateUrl: './case.component.html',
  styleUrls: ['./case.component.scss'],
})
export class CaseComponent implements OnInit {
  isExpanded: Record<string, boolean> = {};

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  _closedCaseDetails: CaseDetail[] | null = [];

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
    private caseManagementService: CaseManagementService,
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
        console.log(closedCases);
      });
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }
}
