import { CUSTOM_ELEMENTS_SCHEMA, Component, OnInit, ViewChild } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Router, RouterLink } from '@angular/router';
import { CaseDetail } from '../shared/api/models';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { MatStepper, MatStep, MatStepLabel, MatStepperIcon } from '@angular/material/stepper';
import { LoginService } from '../shared/services/login.service';
import { DatePipe } from '@angular/common';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';

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
    standalone: true,
    imports: [
        MatCard,
        RouterLink,
        MatCardContent,
        MatStepper,
        MatStep,
        MatStepLabel,
        MatStepperIcon,
        MatIcon,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        DatePipe,
        MatAccordion
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class RecentCaseComponent implements OnInit {
  public caseDetails: CaseDetail | undefined;

  selectedIndex = 0;
  panelOpenState = false;


  @ViewChild('stepper') stepper!: MatStepper;

  constructor(
    private caseManagementService: CaseManagementService,
    private loginService: LoginService,
    private breakpointObserver: BreakpointObserver,
    private router: Router
  ) {}

  // eslint-disable-next-line @angular-eslint/use-lifecycle-interface
  ngAfterViewInit(): void {
    this.breakpointObserver
      .observe(['(max-width: 768px)'])
      .subscribe((result) => {
        if (result.matches) {
          // this.stepper._stepsList.toArray()[this.selectedIndex].expanded = true;
          this.stepper.orientation = 'vertical';
        } else {
          this.stepper.orientation = 'horizontal';
        }
      });
  }

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
