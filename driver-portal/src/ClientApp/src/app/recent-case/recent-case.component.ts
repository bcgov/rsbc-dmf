import { Component, OnInit, ViewChild } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router } from '@angular/router';
import { CaseDetail } from '../shared/api/models';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { MatStepper } from '@angular/material/stepper';
import { LoginService } from '../shared/services/login.service';

@Component({
  selector: 'app-recent-case',
  templateUrl: './recent-case.component.html',
  styleUrls: ['./recent-case.component.scss'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { displayDefaultIndicatorType: false },
    },
  ],
})
export class RecentCaseComponent implements OnInit {
  public caseDetails: CaseDetail | undefined;

  selectedIndex = 0;

  @ViewChild('stepper') stepper!: MatStepper;

  constructor(
    private caseManagementService: CaseManagementService,
    private loginService: LoginService,
    private router: Router
  ) {}

  public ngOnInit(): void {
    this.caseManagementService
      .getMostRecentCase(this.loginService.userProfile?.id as string)
      .subscribe((recentCase) => {
        this.caseDetails = recentCase;
        if (recentCase.status === 'Opened') {
          this.selectedIndex = 0;
        }
        if (recentCase.status === 'Open Pending Submission') {
          this.selectedIndex = 1;
        }
        if (recentCase.status === 'Under Review') {
          this.selectedIndex = 2;
        }
        if (recentCase.status === 'File End Tasks') {
          this.selectedIndex = 3;
        }
        if (recentCase.status === 'Intake Validation') {
          this.selectedIndex = 4;
        }
        if (recentCase.status === 'Closed') {
          this.selectedIndex = 5;
        }
      });
  }
}
