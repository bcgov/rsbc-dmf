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

@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.css'],
})
export class SubmissionRequirementsComponent {
  @Input() submissionRequirementDocuments?: Document[] | null = [];
  @Output() viewLetter = new EventEmitter<string>();

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  fileToUpload: File | null = null;

  constructor(private caseManagementService: CaseManagementService) {}

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
    this.fileToUpload = event.target.files.item(0);
    this.fileUpload();
  }

  fileUpload() {
    console.log('fileUpload');
    this.caseManagementService
      .getUploadDocuments({ body: this.fileToUpload as any })
      .subscribe((res) => {
        console.log(res);
      });
  }
}
