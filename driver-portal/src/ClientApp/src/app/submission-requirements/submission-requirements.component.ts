import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document, DocumentSubType } from '../shared/api/models';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { ApiConfiguration } from '../shared/api/api-configuration';

interface DocumentType {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.scss'],
})
export class SubmissionRequirementsComponent implements OnInit {
  @Input() submissionRequirementDocuments?: Document[] | null = [];
  @Output() viewLetter = new EventEmitter<string>();

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  fileToUpload: File | null = null;
  documentSubTypes?: DocumentSubType[];
  acceptControl = new FormControl(false);

  // documentTypes: DocumentType[] = [
  //   { value: '310', viewValue: 'Diabetic Doctor Report' },
  //   { value: '001', viewValue: 'DMER' },
  //   { value: '030', viewValue: 'EVF' },
  // ];

  constructor(
    private caseManagementService: CaseManagementService,
    private _http: HttpClient,
    private _snackBar: MatSnackBar,
    public dialog: MatDialog,
    private apiConfig: ApiConfiguration
  ) {}

  ngOnInit() {
    //console.log('submission requirements component');
    this.getDocumentSubtypes();
  }

  getDocumentSubtypes() {
    this.caseManagementService.getDocumentSubTypes({}).subscribe((response) => {
      this.documentSubTypes = response;
    });
  }
  public files: any[] = [];

  onSelect(event: any) {
    console.log(event);
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
      duration: 2000,
    });
  }

  show = false;

  openUploadFile() {
    console.log('openUploadFile');
    this.show = true;
  }

  closeUploadFile() {
    console.log('closeUploadFile');
    this.show = false;
  }

  handleFileInput(event: any) {
    console.log('handleFileInput', event);
    this.fileToUpload = event.target.files[0];
  }

  fileUpload() {
    console.log('fileUpload');
    // this.caseManagementService
    //   .({ body: { file: this.fileToUpload } as any })
    //   .subscribe((res) => {
    //     console.log(res);
    //   });
    if (!this.fileToUpload) {
      console.log('No file selected');
      return;
    }

    const formData = new FormData();
    formData.append('file', this.fileToUpload as File);
    

    this._http
      .post(`${this.apiConfig.rootUrl}/api/Document/upload`, formData, {
        headers: {
          enctype: 'multipart/form-data',
        },
      })
      .subscribe((res) => {
        console.log(res);
        this._snackBar.open('Successfully uploaded!', 'Close', {});
      });
  }
}
