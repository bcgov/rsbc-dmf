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
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.css'],
})
export class SubmissionRequirementsComponent {
  @Input() submissionRequirementDocuments?: Document[] | null = [];
  @Output() viewLetter = new EventEmitter<string>();

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  fileToUpload: Blob | string = "";

  constructor(private caseManagementService: CaseManagementService, private _http: HttpClient) {}

  show = false;

  // navigateToLetters() {
  //   this.viewLetter.emit();
  // }

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

    const formData = new FormData();
    formData.append("file", this.fileToUpload);

    this._http.post("/api/Document/upload", formData).subscribe({
      next: (event) => {
        console.log(event);
      }
    });
  }
}
