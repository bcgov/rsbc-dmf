import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
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
    @Inject(MAT_DIALOG_DATA)
    private data: CallbackCancelRequest
  ) {}

  cancelRequestCallback() {
    this.caseManagementService
      .cancelCallBackRequest({ body: this.data })

      .subscribe((res) => {
        console.log(res);
        console.log(this.data);
      });
  }

  onCancel() {
    this.dialogRef.close();
  }
}
