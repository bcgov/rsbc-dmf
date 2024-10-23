import { Component, Inject, Input } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogTitle,
  MatDialogClose,
  MatDialogActions,
} from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { MatIconButton, MatButton } from '@angular/material/button';


@Component({
  selector: 'app-shared-cancel-callback-dialog',
  templateUrl: './cancel-callback-dialog.component.html',
  styleUrls: ['./cancel-callback-dialog.component.scss'],
  standalone: true,
  imports: [
    MatDialogTitle,
    MatIconButton,
    MatDialogClose,
    MatIcon,
    MatCard,
    MatCardContent,
    MatDialogActions,
    MatButton,
  ],
})
export class SharedCancelCallbackDialogComponent {


  constructor(
    private dialogRef: MatDialogRef<SharedCancelCallbackDialogComponent>,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA)
    private data: any,
  ) {}

  iscancelCallback = false;

  cancelRequestCallback() {
    if (this.iscancelCallback) {
      return;
    }
    this.iscancelCallback = true;
    this.data.caseManagementService
      .cancelCallBackRequest({ body: this.data })
      .subscribe(() => {
        this.dialogRef.close();
        this.iscancelCallback = false;
        this._snackBar.open(
          'Successfully cancelled callback request',
          'Close',
          {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 5000,
          },
        );
      });
  }

  onCancel() {
    this.dialogRef.close();
  }
}
