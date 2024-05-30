import { Component, Input } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { MatButton } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CaseDocument } from '@app/shared/api/models';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-submission-requirements',
  standalone: true,
  imports: [QuickLinksComponent, MatButton, MatCardModule],
  providers: [DatePipe],
  templateUrl: './submission-requirements.component.html',
  styleUrl: './submission-requirements.component.scss',
})
export class SubmissionRequirementsComponent {
  fileToUpload: File | null = null;

  constructor(
    private _snackBar: MatSnackBar,
    private fb: FormBuilder,
    private datePipe: DatePipe
  ) {}
  public files: any[] = [];
  acceptControl = new FormControl(false);
  @Input() documents: CaseDocument[] = [];

  getFormattedDate(date: string | undefined | null) {
    if (!date) {
      return ' ';
    }
    return this.datePipe.transform(date, 'longDate');
  }

  uploadForm = this.fb.group({
    documentSubType: ['', Validators.required],
  });

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
    // if (this.isFileUploading) {
    //   return;
    // }
    // if (!this.fileToUpload) {
    //   this._snackBar.open('Please select the file to Upload', 'Close', {
    //     horizontalPosition: 'center',
    //     verticalPosition: 'top',
    //     duration: 5000,
    //   });
    //   return;
    // }
    // const formData = new FormData();
    // formData.append('file', this.fileToUpload as File);
    // //formData.append('documentSubTypeId', this.uploadForm.controls.documentSubType.value as any);
    // this.isFileUploading = true;
    // this._http
    // .post(`${this.apiConfig.rootUrl}/api/Document/upload`, formData, {
    //   })
    //   .subscribe(() => {
    //     this.fileToUpload = null;
    //     //this.uploadForm.controls.documentSubType.setValue('');
    //     //this.acceptControl.reset();
    //     this._snackBar.open('Successfully uploaded!', 'Close', {
    //       horizontalPosition: 'center',
    //       verticalPosition: 'top',
    //       duration: 5000,
    //     });
    //     this.showUpload = false;
    //     this.isFileUploading = false;
    //   });
  }
}
