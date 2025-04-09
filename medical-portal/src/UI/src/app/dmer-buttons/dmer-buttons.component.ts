import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCommonModule } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { DMERStatusEnum } from '@app/app.model';
import { ClaimDmerPopupComponent } from '@app/claim-dmer-popup/claim-dmer-popup.component';
import { Role } from '@app/features/auth/enums/role.enum';
import { PopupService } from '@app/popup/popup.service';
import { PatientCase } from '@app/shared/api/models';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-dmer-buttons',
  standalone: true,
  imports: [
    MatCommonModule,
    MatInputModule,
    MatButtonModule,
    ClaimDmerPopupComponent
  ],
  templateUrl: './dmer-buttons.component.html',
  styleUrl: './dmer-buttons.component.scss'
})
export class DmerButtonsComponent {

  public accessLevel: Role = Role.Moa;
  @Input() public searchedCase?: PatientCase | null;
  DMERStatusEnum = DMERStatusEnum;
  @Output() public popupClosed = new EventEmitter();

  constructor(
    private profileManagementService: ProfileManagementService,
    private dialog: MatDialog,
    private popupService: PopupService,
    private _snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
    let profile = this.profileManagementService.getCachedProfile();
    // TODO will be using "accessLevel" in other areas, move to profile service
    this.accessLevel = profile.roles?.find((role) => role === Role.Practitioner) ? Role.Practitioner : Role.Moa;
  }

  openPopup() {
    if (!this.searchedCase) {
      console.error('Case data was missing', this.searchedCase);
      return;
    }
    this.popupService
      .openPopup(this.searchedCase.caseId as string, this.searchedCase.documentId as string)
      .subscribe((event) => {
        console.log('Popup closed', event);
        const snackbarRef = this._snackBar.open(
          'Successfully Submitted the Chefs Form',
          'Close',
          {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 8000,
          }
        );

        snackbarRef.afterDismissed().subscribe(() => {
          this.popupClosed.emit();
        });
      });
  }

  openClaimPopup(documentId?: string | null) {
    const dialogRef = this.dialog.open(ClaimDmerPopupComponent, {
      height: '600px',
      width: '820px',
      data: documentId,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.popupClosed.emit();
    });
  }

  public get Role() {
    return Role;
  }
}
