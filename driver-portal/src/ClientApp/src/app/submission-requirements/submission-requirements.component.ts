import { Component, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';

@Component({
  selector: 'app-submission-requirements',
  templateUrl: './submission-requirements.component.html',
  styleUrls: ['./submission-requirements.component.css'],
})
export class SubmissionRequirementsComponent {
  @ViewChild(MatAccordion) accordion!: MatAccordion;
}
