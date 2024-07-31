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
import { CaseStatusComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-status/case-status.component';
import { CaseTypeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/case-type/case-type.component';
import { MedicalDmerTypesComponent } from '../../app/definitions/medical-dmer-types/medical-dmer-types.component';
import { DecisionOutcomeComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/decision-outcome/decision-outcome.component';
import { EligibleLicenseClassComponent } from '../../../../../../shared-portal-ui/projects/core-ui/src/lib/case-definitions/eligible-license-class/eligible-license-class.component';
import { RecentCaseComponent } from '../../app/recent-case/recent-case.component';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { CasesService } from '@app/shared/api/services';
import { CaseDetail } from '@app/shared/api/models';
import { MatIcon } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CommentsComponent } from '@app/comments/comments.component';

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
    MatIcon,
    NgIf,
    MatDialogModule,
    CommentsComponent,
  ],
  templateUrl: './driver-search.component.html',
  styleUrl: './driver-search.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DriverSearchComponent implements OnInit {
  readonly dialog = inject(MatDialog);

  isExpanded: Record<string, boolean> = {};

  isLoading = true;

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  driverId = '';

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

  constructor(private caseService: CasesService) {}

  ngOnInit(): void {
    if (this.driverId) {
      this.getClosedCases(this.driverId as string);
    } else {
      console.log('No user profile');
    }
  }

  getClosedCases(driverId: string) {
    this.caseService
      .apiCasesClosedGet$Json({ driverId })
      .subscribe((closedCases: any) => {
        this.closedCaseDetails = closedCases;
        this.isLoading = false;
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
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
    });
  }
}
