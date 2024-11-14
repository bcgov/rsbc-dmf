import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { map, Observable, tap } from 'rxjs';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';

/*
  To call this service from one of the portals, use code similar to the following:

  openConfirmationDialog() {
    this.confirmationDialogService.openDialog('350px', '220px', 'Are you sure you want to submit the form?', 'Submit', 'Cancel').subscribe((isConfirmed: boolean) => {
      console.log("isConfirmed", isConfirmed);
    });
  }
*/
@Injectable({
  providedIn: 'root',
})
export class ConfirmationDialogService {
  dialogRef!: MatDialogRef<ConfirmationDialogComponent>;

  constructor(private dialog: MatDialog) {}

  openDialog(width: string = '350px', height: string = '220px', confirmMessage: string = 'Are you sure?', confirmButtonText: string = 'Ok', cancelButtonText: string = 'Cancel'): Observable<boolean> {
    this.dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: width,
      height: height
    });
    this.dialogRef.componentInstance.confirmMessage = confirmMessage;
    this.dialogRef.componentInstance.confirmButtonText = confirmButtonText;
    this.dialogRef.componentInstance.cancelButtonText = cancelButtonText;

    return this.dialogRef.afterClosed().pipe(map((result) => result as boolean));
  }

  closePopup() {
    this.dialog.closeAll();
  }
}
