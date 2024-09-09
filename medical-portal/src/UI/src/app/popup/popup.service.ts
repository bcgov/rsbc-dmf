import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PopupComponent } from './popup.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PopupService {
  constructor(private dialog: MatDialog) {}

  openPopup(caseId: string, documentId: string): Observable<any> {
    const dialogRef = this.dialog.open(PopupComponent, {
      data: { caseId, documentId },
      width: '80vw',
      maxWidth: '80vw',
    });

    return dialogRef.afterClosed();
  }

  closePopup() {
    this.dialog.closeAll();
  }
}
