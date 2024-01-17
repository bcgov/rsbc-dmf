import { Component, OnInit, ViewChild } from '@angular/core';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router } from '@angular/router';
import { CaseDetail } from '../shared/api/models';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { MatStepper } from '@angular/material/stepper';

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

  @ViewChild('stepper') stepper!: MatStepper;

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router
  ) {}

  public ngOnInit(): void {
    this.caseManagementService.getMostRecentCase({}).subscribe((recentCase) => {
      this.caseDetails = recentCase;
      //this.stepper.selectedIndex = 2;
    });
  }
}
