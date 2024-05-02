import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import {
  MatLabel,
  MatError,
  MatFormFieldModule,
} from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import {MatCheckboxModule} from '@angular/material/checkbox'
//import { HttpClient } from '@angular/common/http';
//import { ApiConfiguration } from '../shared/api/api-configuration';

@Component({
  selector: 'app-upload-document',
  standalone: true,
  imports: [
    MatButtonModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatCheckboxModule,
    MatLabel,
    MatSelectModule,
    MatOptionModule,
    MatError,
    NgxDropzoneModule,
    MatIconModule,
  ],
  templateUrl: './upload-document.component.html',
  styleUrl: './upload-document.component.scss',
})
export class UploadDocumentComponent {
  fileToUpload: File | null = null;

  constructor(private _snackBar: MatSnackBar,  private fb: FormBuilder) {}
  public files: any[] = [];
  acceptControl = new FormControl(false);

  uploadForm = this.fb.group({
    documentSubType : ['', Validators.required],

  })

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
