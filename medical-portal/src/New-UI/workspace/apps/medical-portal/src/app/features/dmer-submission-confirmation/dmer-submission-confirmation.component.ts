/* eslint-disable @typescript-eslint/explicit-function-return-type */

/* eslint-disable @typescript-eslint/explicit-member-accessibility */
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';

import { DmerSubmissionConfirmationDialogComponent } from './dmer-submission-confirmation-dialog/dmer-submission-confirmation-dialog.component';

@Component({
  selector: 'app-dmer-submission-confirmation',
  templateUrl: './dmer-submission-confirmation.component.html',
  styleUrls: ['./dmer-submission-confirmation.component.scss'],
})
export class DmerSubmissionConfirmationComponent {
  constructor(public dialog: MatDialog, private router: Router) {}

  openConfirmationDialog(action: string) {
    const dialogRef = this.dialog.open(
      DmerSubmissionConfirmationDialogComponent,
      {
        height: '300px',
        width: '820px',
        // data
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      console.log('Dialog result:', result);
      if (result) {
        if (action === 'dashboard') {
          this.router.navigate(['/dashboard']);
        } else if (action === 'logout') {
          //this.loginService.logout()
        }
      }
    });
  }
}
