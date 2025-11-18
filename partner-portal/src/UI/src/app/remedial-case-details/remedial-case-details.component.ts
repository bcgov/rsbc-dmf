import { Component, CUSTOM_ELEMENTS_SCHEMA, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { RecentCaseComponent, CaseStatusComponent, CaseTypeComponent, DecisionOutcomeComponent, EligibleLicenseClassComponent, PortalsEnum } from '@shared/core-ui';
import { MedicalDmerTypesComponent } from '../../app/definitions/medical-dmer-types/medical-dmer-types.component';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { UserService } from '@app/shared/services/user.service';
import { CaseDetail } from '@app/shared/api/models';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
@Component({
  selector: 'app-remedial-case-details',
  standalone: true,
  imports: [
    RecentCaseComponent,
    MatExpansionModule,
    CaseStatusComponent,
    CaseTypeComponent,
    DecisionOutcomeComponent,
    EligibleLicenseClassComponent,
    MedicalDmerTypesComponent,
    DatePipe,
    NgIf,
    NgFor,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './remedial-case-details.component.html',
  styleUrl: './remedial-case-details.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class RemedialCaseDetailsComponent implements OnInit {

  isExpanded: Record<string, boolean> = {};
  PortalsEnum = PortalsEnum;
  isLoading = true;
  @ViewChild(MatAccordion) accordion!: MatAccordion;



  // Get Driver details
  driverDetails = this.userService.getCachedriver();

  constructor(
    public caseManagementService: CaseManagementService,
    public userService: UserService
  ) { }

  ngOnInit(): void {
    if (this.driverDetails.id) {
      this.getClosedRemedialCases(this.driverDetails.id as string);
    } else {
      console.log('No user profile');
    }
  }
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

  getClosedRemedialCases(driverId: string, programArea: string = 'Remedial') {

    this.caseManagementService.getClosedCases({ programArea: programArea }).subscribe({
      next: (closedCases) => {
        this.closedCaseDetails = closedCases;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error fetching remedial Closed Cases:', error);
      },
    });
  }


  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

}
