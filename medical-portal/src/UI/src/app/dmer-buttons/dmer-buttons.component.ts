import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DMERStatusEnum } from '@app/app.model';
import { Role } from '@app/features/auth/enums/identity-provider.enum';
import { PopupService } from '@app/popup/popup.service';
import { PatientCase } from '@app/shared/api/models';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { ClaimDmerPopupComponent } from '@src/claim-dmer-popup/claim-dmer-popup.component';

@Component({
  selector: 'app-dmer-buttons',
  standalone: true,
  imports: [],
  templateUrl: './dmer-buttons.component.html',
  styleUrl: './dmer-buttons.component.scss'
})
export class DmerButtonsComponent {
  public role?: Role;
  DMERStatusEnum = DMERStatusEnum;
  @Input() searchedCase?: PatientCase;
  @Output() afterDialogClosed = new EventEmitter<void>();

  constructor(private profileManagementService: ProfileManagementService, private popupService: PopupService, private dialog: MatDialog) {
    this.role = this.profileManagementService.getCachedProfile().roles?.find((role) => role === Role.Practitioner)
      ? Role.Practitioner
      : Role.Moa;
  }

  public get Role() {
    return Role;
  }

  openPopup() {
    if (!this.searchedCase) {
      console.error('Case data was missing', this.searchedCase);
      return;
    }
    this.popupService.openPopup(this.searchedCase.caseId as string, this.searchedCase.documentId as string);
  }

  openClaimPopup(searchedCase: PatientCase) {
    const dialogRef = this.dialog.open(ClaimDmerPopupComponent, {
      height: '600px',
      width: '820px',
      data: searchedCase,
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.afterDialogClosed.emit();
      console.log('The dialog was closed', result);
    });
  }
}
