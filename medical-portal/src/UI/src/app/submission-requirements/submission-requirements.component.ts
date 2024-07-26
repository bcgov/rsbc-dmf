import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormField, MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { MatButton } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CaseDocument, DocumentSubTypes } from '@app/shared/api/models';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ApiConfiguration } from '@app/shared/api/api-configuration';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DocumentTypeService } from '@app/shared/api/services';
import { MatIcon } from '@angular/material/icon';
import { PopupService } from '@app/popup/popup.service';

@Component({
  selector: 'app-submission-requirements',
  standalone: true,
  imports: [
    QuickLinksComponent,
    MatButton,
    MatCardModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    ReactiveFormsModule,
    NgxDropzoneModule,
    MatIcon,
  ],
  providers: [DatePipe],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './submission-requirements.component.html',
  styleUrl: './submission-requirements.component.scss',
})
export class SubmissionRequirementsComponent {
  fileToUpload: File | null = null;

  constructor(
    private _snackBar: MatSnackBar,
    private fb: FormBuilder,
    private _http: HttpClient,
    private datePipe: DatePipe,
    private apiConfig: ApiConfiguration,
    private documentTypeService: DocumentTypeService,
    private popupService: PopupService,
  ) {}
  public files: any[] = [];
  acceptControl = new FormControl(false);
  @Input() documents: CaseDocument[] = [];
  @Input() driverId?: string | null;
  @Output() uploadedDocument = new EventEmitter();
  @Input() isLoading = true;

  documentSubTypes?: DocumentSubTypes[];

  uploadForm = this.fb.group({
    documentSubType: ['', Validators.required],
  });

  ngOnInit() {
    this.getDocumentSubtypes();
  }

  getDocumentSubtypes() {
    this.documentTypeService
      .apiDocumentTypeDocumentSubTypeGet$Json({})
      .subscribe((response) => {
        this.documentSubTypes = response;
      });
  }

  getFormattedDate(date: string | undefined | null) {
    if (!date) {
      return ' ';
    }
    return this.datePipe.transform(date, 'longDate');
  }

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
    formData.append(
      'documentSubTypeId',
      this.uploadForm.controls.documentSubType.value as any,
    );
    formData.append('driverId', this.driverId as string);
    this.isFileUploading = true;
    this._http
      .post(`${this.apiConfig.rootUrl}/api/Document/upload`, formData, {})
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
        this.uploadedDocument.emit();
      });
  }
}
