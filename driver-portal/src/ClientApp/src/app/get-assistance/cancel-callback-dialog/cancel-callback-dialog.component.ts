import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-cancel-callback-dialog',
  templateUrl: './cancel-callback-dialog.component.html',
  styleUrls: ['./cancel-callback-dialog.component.scss'],
})
export class CancelCallbackDialogComponent {
  constructor(private dialogRef: MatDialogRef<CancelCallbackDialogComponent>) {}

  cancelRequestCallback() {
    console.log('Cancel Request');
  }

  onCancel() {
    this.dialogRef.close();
  }
}
