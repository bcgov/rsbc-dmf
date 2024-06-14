import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PopupComponent } from './popup.component';

@Injectable({
  providedIn: 'root',
})
export class PopupService {
  constructor(private dialog: MatDialog) {}

  openPopup() {
    this.dialog.open(PopupComponent, { width: '80vw', maxWidth: '80vw' });
  }

  closePopup() {
    this.dialog.closeAll();
  }
}
