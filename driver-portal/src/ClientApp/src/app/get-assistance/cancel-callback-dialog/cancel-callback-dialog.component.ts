import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CallbackCancelRequest } from 'src/app/shared/api/models';
import { CaseManagementService } from 'src/app/shared/services/case-management/case-management.service';

@Component({
  selector: 'app-cancel-callback-dialog',
  templateUrl: './cancel-callback-dialog.component.html',
  styleUrls: ['./cancel-callback-dialog.component.scss'],
})
export class CancelCallbackDialogComponent {
  constructor(
    private dialogRef: MatDialogRef<CancelCallbackDialogComponent>,
    private caseManagementService: CaseManagementService,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA)
    private data: CallbackCancelRequest
  ) {}

  iscancelCallback = false;

  cancelRequestCallback() {
    if (this.iscancelCallback) {
      return;
    }
    this.iscancelCallback = true;
    this.caseManagementService
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
          }
        );
      });
  }

  onCancel() {
    this.dialogRef.close();
  }
}
