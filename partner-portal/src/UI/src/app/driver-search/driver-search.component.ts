import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  Input,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatToolbar } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MedicalDmerTypesComponent } from '../../app/definitions/medical-dmer-types/medical-dmer-types.component';
import { RecentCaseComponent, CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, EligibleLicenseClassComponent, PortalsEnum } from '@shared/core-ui';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { CaseDetail } from '@app/shared/api/models';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CommentsComponent } from '@app/comments/comments.component';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';
import { MatTabsModule } from '@angular/material/tabs';
import { LettersToDriverComponent } from '@app/letters-to-driver/letters-to-driver.component';
import { GetAssistanceComponent } from '@app/get-assistance/get-assistance.component';
import { SubmissionHistoryComponent } from '@app/submission-history/submission-history.component';
import { MatButtonModule } from '@angular/material/button';


@Component({
  selector: 'app-driver-search',
  standalone: true,
  imports: [
    MatCardModule,
    MatToolbar,
    RouterLink,
    MatExpansionModule,
    CaseStatusComponent,
    CaseTypeComponent,
    MedicalDmerTypesComponent,
    DecisionOutcomeComponent,
    EligibleLicenseClassComponent,
    RecentCaseComponent,
    DatePipe,
    NgFor,
    NgIf,
    CommentsComponent,
    MatTabsModule,
    LettersToDriverComponent,
    GetAssistanceComponent,
    SubmissionHistoryComponent,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './driver-search.component.html',
  styleUrl: './driver-search.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DriverSearchComponent implements OnInit {
  readonly dialog = inject(MatDialog);

  isExpanded: Record<string, boolean> = {};

  isLoading = true;

  PortalsEnum = PortalsEnum;

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  _closedCaseDetails: CaseDetail[] | null = [];
  driverLicenceNumber = '';

   // Get Driver details
   driverDetails = this.userService.getCachedriver();

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
     private userService: UserService,
     ) {}

  ngOnInit(): void {
    if (this.driverDetails.id) {
      this.getClosedCases(this.driverDetails.id as string);
    } else {
      console.log('No user profile');
    }

  this.search();
  }

  getClosedCases(driverId: string) {
    this.caseManagementService
      .getClosedCases({ driverId })
      .subscribe((closedCases: any) => {
        this.closedCaseDetails = closedCases;
        this.isLoading = false;
      });
  }


  search() {
    this.caseManagementService
      .searchByDriver({ driverLicenceNumber: this.driverLicenceNumber })
      .subscribe({
        next: (driver) => {
          this.userService.setCacheDriver(driver);
          this.driverDetails = this.driverDetails;
        },
        error: (error) => {
         
          console.error('error', error);
        }
      });
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  openCommentsDialog() {
    const dialogRef = this.dialog.open(CommentsComponent, {
      height: '730px',
      width: '400px',
      position: {
        bottom: '8px',
        right: '8px',
      },
      data: this.driverDetails.driverId
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
    });
  }
}
