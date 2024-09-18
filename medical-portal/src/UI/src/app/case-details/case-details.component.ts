import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  Input,
  OnInit,
  ViewChild,
  input,
} from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import {
  MatAccordion,
  MatExpansionModule,
  MatExpansionPanel,
} from '@angular/material/expansion';
import {
  CaseStatusComponent,
  CaseTypeComponent,
  DecisionOutcomeComponent,
  DmerStatusComponent,
  EligibleLicenseClassComponent,
  LetterTopicComponent,
  SubmissionStatusComponent,
  SubmissionTypeComponent,
  UploadDocumentComponent,
} from '@shared/core-ui';

import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { CaseSubmissionsComponent } from '../case-submissions/case-submissions.component';
import { SubmissionRequirementsComponent } from '../submission-requirements/submission-requirements.component';
import { CasesService, DocumentService } from '@app/shared/api/services';
import { CaseDocument, PatientCase } from '@app/shared/api/models';
import {
  CaseStageEnum,
  DMERStatusEnum,
  DocumentTypeEnum,
  SubmittalStatusEnum,
} from '@app/app.model';
import { DatePipe } from '@angular/common';
import { MedicalDmerTypesComponent } from '@app/definitions/medical-dmer-types/medical-dmer-types.component';
import { PopupService } from '../popup/popup.service';
import { DmerButtonsComponent } from "../dmer-buttons/dmer-buttons.component";

@Component({
  selector: 'app-case-details',
  standalone: true,
  imports: [
    MatCardModule,
    MatStepperModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatExpansionModule,
    CaseTypeComponent,
    CaseStatusComponent,
    MedicalDmerTypesComponent,
    DmerStatusComponent,
    DecisionOutcomeComponent,
    EligibleLicenseClassComponent,
    SubmissionTypeComponent,
    SubmissionStatusComponent,
    LetterTopicComponent,
    UploadDocumentComponent,
    MatTabsModule,
    CaseSubmissionsComponent,
    SubmissionRequirementsComponent,
    DatePipe,
    DmerButtonsComponent
],
  templateUrl: './case-details.component.html',
  styleUrl: './case-details.component.scss',
  viewProviders: [MatExpansionPanel],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { displayDefaultIndicatorType: false },
    },
  ],
})
export class CaseDetailsComponent implements OnInit {
  idCode = input<string>();
  caseId = input<string>();
  caseDetails?: PatientCase;

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  selectedIndex = 0;
  pageSize = 10;
  @ViewChild('stepper') stepper!: MatStepper;

  filteredDocuments?: CaseDocument[] | null = [];

  allDocuments: CaseDocument[] = [];
  submissionRequirementDocuments: CaseDocument[] = [];
  driverSubmissionDocuments: CaseDocument[] = [];
  isLoading = true;
  DMERStatusEnum = DMERStatusEnum;

  constructor(
    private breakpointObserver: BreakpointObserver,
    private casesService: CasesService,
    private documentService: DocumentService,
    private popupService: PopupService,
  ) { }

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
    this.casesService
      .apiCasesSearchIdCodeGet$Json({ idCode: this.idCode() as string })
      .subscribe((caseDetails) => {
        console.log(this.caseDetails);
        this.caseDetails = caseDetails;
        if (caseDetails?.status === CaseStageEnum.Opened) {
          this.selectedIndex = 0;
        }
        if (caseDetails.status === CaseStageEnum.OpenPendingSubmission) {
          this.selectedIndex = 1;
        }
        if (caseDetails.status === CaseStageEnum.UnderReview) {
          this.selectedIndex = 2;
        }
        if (caseDetails.status === CaseStageEnum.FileEndTasks) {
          this.selectedIndex = 3;
        }
        if (caseDetails.status === CaseStageEnum.IntakeValidation) {
          this.selectedIndex = 4;
        }
        if (caseDetails.status === CaseStageEnum.Closed) {
          this.selectedIndex = 5;
        }

        // Load docuemnts
        this.getDriverDocuments(this.caseDetails?.driverId as string);
      });
  }

  getDriverDocuments(driverId: string) {
    this.submissionRequirementDocuments = [];
    this.driverSubmissionDocuments = [];

    this.documentService
      .apiDocumentDriverIdAllDocumentsGet$Json({
        driverId: driverId,
      })
      .subscribe((documents) => {
        if (!documents) {
          return;
        }
        this.allDocuments = documents;

        const submissionRequirementDocuments: CaseDocument[] = [];
        const driverSubmissionDocuments: CaseDocument[] = [];
        documents.forEach((doc) => {
          if (
            [SubmittalStatusEnum.OpenRequired].includes(
              doc.submittalStatus as SubmittalStatusEnum,
            )
          ) {
            submissionRequirementDocuments.push(doc);
          } else if (
            ![
              SubmittalStatusEnum.OpenRequired,
              SubmittalStatusEnum.Issued,
              SubmittalStatusEnum.Sent,
            ].includes(doc.submittalStatus as SubmittalStatusEnum)
          ) {
            driverSubmissionDocuments.push(doc);
          }
        });

        this.submissionRequirementDocuments = submissionRequirementDocuments;
        this.driverSubmissionDocuments = driverSubmissionDocuments;
        this.isLoading = false;
      });
  }

  onUploadDocument() {
    // Refresh the documents tab after uploading a document
    this.getDriverDocuments(this.caseDetails?.driverId as string);
  }

  openPopup() {
    if (!this.caseDetails) {
      console.error('Case data was missing', this.caseDetails);
      return;
    }
    this.popupService.openPopup(this.caseId() as string, this.caseDetails.documentId as string);
  }
}
