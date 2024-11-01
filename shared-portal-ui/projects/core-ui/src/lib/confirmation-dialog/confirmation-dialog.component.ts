import { Component, ElementRef, Inject, ViewChild, TemplateRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogActions, MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [MatDialogActions, MatIcon],
  templateUrl: './confirmation-dialog.component.html',
  styleUrl: './confirmation-dialog.component.scss',
})
export class ConfirmationDialogComponent {
  public title: string = "Confirm";
  public confirmMessage: string = 'Are you sure?';
  public confirmButtonText: string = 'Ok';
  public cancelButtonText: string = 'Cancel';

  constructor(
    //@Inject(MAT_DIALOG_DATA) public data: { caseId: string, documentId: string },
    public dialogRef: MatDialogRef<ConfirmationDialogComponent>
  ) {
      //this.caseId = data.caseId;
      //this.documentId = data.documentId;
  }
}
