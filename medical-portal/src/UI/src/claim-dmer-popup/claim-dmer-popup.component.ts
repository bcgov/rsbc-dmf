import { Component, Inject } from '@angular/core';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { DocumentService } from '@app/shared/api/services';
import { PatientCase } from '@app/shared/api/models';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-claim-dmer-popup',
  standalone: true,
  imports: [
    MatIconModule,
    MatDialogModule,
    MatRadioModule,
    MatButtonModule,
    MatSelectModule,
    MatCheckboxModule,
  ],
  templateUrl: './claim-dmer-popup.component.html',
  styleUrl: './claim-dmer-popup.component.scss',
})
export class ClaimDmerPopupComponent {
  constructor(
    private documentService: DocumentService,
    @Inject(MAT_DIALOG_DATA) public data: PatientCase,
    @Inject(MatDialogRef<ClaimDmerPopupComponent>)
    private dialogRef: MatDialogRef<ClaimDmerPopupComponent>,
    private _snackBar: MatSnackBar,
  ) {}

  onClaimDmer() {
    this.documentService
      .apiDocumentClaimDmerPost$Json({
        documentId: this.data.documentId as string,
      })
      .subscribe(() => {
        this._snackBar.open('Successfully Claimed the DMER', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
        });
        this.dialogRef.close();
      });
  }

  onUnclaimDmer() {
    this.documentService
      .apiDocumentUnclaimDmerPost$Json({
        documentId: this.data.documentId as string,
      })
      .subscribe(() => {
        this._snackBar.open('Successfully Unclaimed the DMER', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
        });
        this.dialogRef.close();
      });
  }
}
