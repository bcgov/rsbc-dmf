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
import { Document } from '../shared/api/models';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';

interface DocumentType {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.scss'],
})
export class SubmissionRequirementsComponent {
  @Input() submissionRequirementDocuments?: Document[] | null = [];
  @Output() viewLetter = new EventEmitter<string>();

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  fileToUpload: File | null = null;
  selectedValue?: string;
  acceptControl = new FormControl(false);

  docuemntTypes: DocumentType[] = [
    { value: '310', viewValue: 'Diabetic Doctor Report' },
    { value: '001', viewValue: 'DMER' },
    { value: '030', viewValue: 'EVF' },
  ];

  constructor(
    private caseManagementService: CaseManagementService,
    private _snackBar: MatSnackBar,
    public dialog: MatDialog
  ) {}
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

  fileUpload() {
    console.log('fileUpload');
    this.caseManagementService
      .getUploadDocuments({ body: { file: this.fileToUpload } as any })
      .subscribe((res) => {
        console.log(res);
      });
  }
}
