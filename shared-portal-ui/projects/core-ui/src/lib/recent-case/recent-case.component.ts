import { CUSTOM_ELEMENTS_SCHEMA, Component, Input, OnInit, ViewChild, input } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Router, RouterLink } from '@angular/router';
// import { CaseDetail } from '../shared/api/models';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { MatStepper, MatStep, MatStepLabel, MatStepperIcon } from '@angular/material/stepper';
import { DatePipe } from '@angular/common';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { CaseStageEnum } from '../app.model';
import{PortalsEnum} from '../app.model';

@Component({
    selector: 'app-shared-recent-case',
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
  public caseDetails?: any; // CaseDetail | undefined;

  @Input() caseManagementService: any;
  @Input() portal!: PortalsEnum;

  PortalsEnum = PortalsEnum;
  hasActiveCase?: boolean;
  

  selectedIndex = 0;
  panelOpenState = false;
  // Partner Portal
  idCode = '';

  @ViewChild('stepper') stepper!: MatStepper;

  constructor(
    // private caseManagementService: CaseManagementService,
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
          if (this.stepper) this.stepper.orientation = 'vertical';
        } else {
          if (this.stepper) this.stepper.orientation = 'horizontal';
        }
      });
  }

  public ngOnInit(): void {
    this.caseManagementService
      .getMostRecentCase()
      .subscribe((recentCase: any) => {
        this.caseDetails = recentCase;
        if (recentCase.status === CaseStageEnum.Opened) {
          this.selectedIndex = 0;
        }
        if (recentCase.status === CaseStageEnum.OpenPendingSubmission) {
          this.selectedIndex = 1;
        }
        if (recentCase.status === CaseStageEnum.UnderReview) {
          this.selectedIndex = 2;
        }
        if (recentCase.status === CaseStageEnum.FileEndTasks) {
          this.selectedIndex = 3;
        }
        if (recentCase.status === CaseStageEnum.IntakeValidation) {
          this.selectedIndex = 4;
        }
        if (recentCase.status === CaseStageEnum.Closed) {
          this.selectedIndex = 5;
        }
        this.hasActiveCase = true;
      },
      (error: any) => {
        this.hasActiveCase = false;
      }
      );
  }


  // partner Portal
  searchByCaseId(){
    this.caseManagementService.searchByCaseId({idCode: this.idCode})
    .subscribe({
      next: (caseDetails: any) => {
        this.router.navigate(['/caseSearch', this.idCode as string], {state: caseDetails});
        this.hasActiveCase = true;
      },
      error: (error: any) => {
        this.hasActiveCase = false;
      }
  
    });
  }
}
