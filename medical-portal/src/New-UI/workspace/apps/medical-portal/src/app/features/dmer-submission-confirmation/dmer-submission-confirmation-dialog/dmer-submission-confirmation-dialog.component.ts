/* eslint-disable @typescript-eslint/no-empty-function */

/* eslint-disable @typescript-eslint/explicit-member-accessibility */
import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-dmer-submission-confirmation-dialog',
  templateUrl: './dmer-submission-confirmation-dialog.component.html',
  styleUrls: ['./dmer-submission-confirmation-dialog.component.scss'],
})
export class DmerSubmissionConfirmationDialogComponent {
  @HostBinding('class') className = 'mat-dialog-container-host';

  constructor() {}
}
