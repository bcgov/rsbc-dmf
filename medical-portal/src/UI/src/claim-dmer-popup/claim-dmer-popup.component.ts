import { Component, Inject, OnInit } from '@angular/core';
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
import { Endorsement, PatientCase } from '@app/shared/api/models';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { Role } from '@app/features/auth/enums/identity-provider.enum';
import { LicenceStatusCode } from '@app/shared/enum/licence-status-code.enum';

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
export class ClaimDmerPopupComponent implements OnInit {
  public practitioners: Endorsement[] = [];
  public selectedPractitioner?: string;

  constructor(
    private documentService: DocumentService,
    @Inject(MAT_DIALOG_DATA) public data: PatientCase,
    @Inject(MatDialogRef<ClaimDmerPopupComponent>)
    private dialogRef: MatDialogRef<ClaimDmerPopupComponent>,
    private _snackBar: MatSnackBar,
    private profileManagementService: ProfileManagementService
  ) { }

  ngOnInit(): void {
    this.practitioners = this.profileManagementService
      .getCachedProfile()
      .endorsements?.filter(e => e.role === Role.Practitioner && e.licences?.some(l => l.statusCode === LicenceStatusCode.Active))
        || [];
 }

 onAssignDmer() {
  this.documentService
    .apiDocumentAssignDmerPost$Json({
      documentId: this.data.documentId as string,
      loginId: this.selectedPractitioner
    })
    .subscribe(() => {
      this._snackBar.open('Successfully assigned the DMER', 'Close', {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 5000,
      });
      this.dialogRef.close();
    });
}

  onClaimDmer() {
    this.documentService
      .apiDocumentClaimDmerPost$Json({
        documentId: this.data.documentId as string,
      })
      .subscribe(() => {
        this._snackBar.open('Successfully claimed the DMER', 'Close', {
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
