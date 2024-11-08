import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Observable, tap } from 'rxjs';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class ConfirmationDialogService {
  dialogRef!: MatDialogRef<ConfirmationDialogComponent>;

  constructor(private dialog: MatDialog) {}

  openDialog(width: string = '350px', height: string = '220px', confirmMessage: string = 'Are you sure?', confirmButtonText: string = 'Ok', cancelButtonText: string = 'Cancel'): Observable<any> {
    this.dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: width,
      height: height
    });
    this.dialogRef.componentInstance.confirmMessage = confirmMessage;
    this.dialogRef.componentInstance.confirmButtonText = confirmButtonText;
    this.dialogRef.componentInstance.cancelButtonText = cancelButtonText;

    return this.dialogRef.afterClosed();
  }

  closePopup() {
    this.dialog.closeAll();
  }
}
