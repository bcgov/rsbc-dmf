import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { Document, DocumentSubType } from '../api';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ApiConfiguration } from '../api/api-configuration';
import { RouterLink } from '@angular/router';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';
import { NgIf, NgFor, DatePipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { CaseStatusComponent } from '../case-definitions/case-status/case-status.component';
import { CaseTypeComponent } from '../case-definitions/case-type/case-type.component';
import { DecisionOutcomeComponent } from '../case-definitions/decision-outcome/decision-outcome.component';
import { DmerTypeComponent } from '../case-definitions/dmer-type/dmer-type.component';
import {SubmissionStatusComponent} from '../case-definitions/submission-status/submission-status.component';
import {SubmissionTypeComponent} from '../case-definitions/submission-type/submission-type.component';
import {SharedQuickLinksComponent} from '../quick-links/quick-links.component'
import { SubmittalStatusEnum,PortalsEnum } from '../app.model';

@Component({
    selector: 'app-shared-submission-requirements',
    templateUrl: './submission-requirements.component.html',
    styleUrls: ['./submission-requirements.component.scss'],
    standalone: true,
    imports: [
        MatButton,
        NgIf,
        ReactiveFormsModule,
        FormsModule,
        MatFormField,
        MatLabel,
        MatSelect,
        MatOption,
        MatError,
        NgxDropzoneModule,
        MatIcon,
        NgFor,
        MatCard,
        MatCardContent,
        RouterLink,
        MatAccordion,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        CaseTypeComponent,
        CaseStatusComponent,
        DmerTypeComponent,
        DecisionOutcomeComponent,
        SubmissionTypeComponent,
        SubmissionStatusComponent,
        DatePipe,
        SharedQuickLinksComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SharedSubmissionRequirementsComponent implements OnInit {
  @Input() submissionRequirementDocuments?: Document[] | null = [];
  @Output() viewLetter = new EventEmitter<string>();
  PortalsEnum = PortalsEnum;

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  fileToUpload: File | null = null;
  documentSubTypes?: DocumentSubType[];
  //selectedValue = '';
  acceptControl = new FormControl(false);
  @Input() isLoading = true;

  @Input() caseManagementService: any;
  @Input()  portal!: PortalsEnum;

  constructor(
    private _http: HttpClient,
    private _snackBar: MatSnackBar,
    public dialog: MatDialog,
    private apiConfig: ApiConfiguration,
    private fb: FormBuilder
  ) {}

  uploadForm = this.fb.group({
    documentSubType : ['', Validators.required],

  })
  ngOnInit() {
    this.getDocumentSubtypes();

    this.getSubmissionRequireDocuments();
  }

  getDocumentSubtypes() {
    this.caseManagementService.getDocumentSubTypes({})
    .subscribe((response:any) => {
    this.documentSubTypes = response;
    });
  }
  public files: any[] = [];

  onSelect(event: any) {
    this.fileToUpload = event.addedFiles[0];
  }

  onRemove() {
    this.fileToUpload = null;
  }

  deleteFile(f: any) {
    this.files = this.files.filter(function (w) {
      return w.name != f.name;
    });
    this._snackBar.open('Successfully delete!', 'Close', {
      horizontalPosition: 'center',
      verticalPosition: 'top',
      duration: 5000,
    });
  }

  showUpload = false;

  openUploadFile() {
    this.showUpload = true;
  }

  closeUploadFile() {
    this.showUpload = false;
  }

  handleFileInput(event: any) {
    this.fileToUpload = event.target.files[0];
  }
  isFileUploading = false;

  fileUpload() {
    if (this.isFileUploading) {
      return;
    }
    if (!this.fileToUpload) {
      this._snackBar.open('Please select the file to Upload', 'Close', {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 5000,
      });
      return;
    }
    const formData = new FormData();
    formData.append('file', this.fileToUpload as File);
    formData.append('documentSubTypeId', this.uploadForm.controls.documentSubType.value as any);
    this.isFileUploading = true;
    this._http
      .post(`${this.apiConfig.rootUrl}/api/Document/upload`, formData, {
        headers: {
          enctype: 'multipart/form-data',
        },
      })
      .subscribe(() => {
        this.fileToUpload = null;
        this.uploadForm.controls.documentSubType.setValue('');
        this.acceptControl.reset();
        this._snackBar.open('Successfully uploaded!', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
        });
        this.showUpload = false;
        this.isFileUploading = false;
      });
  }

  getSubmissionRequireDocuments() {
    this.caseManagementService
      .getAllDriverDocuments()
      .subscribe((submissiondocs: any) => {
        if (!submissiondocs) {
          return;
        }
        this.submissionRequirementDocuments = [];
        submissiondocs.forEach((doc: any) => {
          if (
            SubmittalStatusEnum.OpenRequired.includes(
              doc.submittalStatus as SubmittalStatusEnum,
            )
          ) {
            this.submissionRequirementDocuments?.push(doc);
          }
        });
        this.isLoading = false;
      });
  }
}