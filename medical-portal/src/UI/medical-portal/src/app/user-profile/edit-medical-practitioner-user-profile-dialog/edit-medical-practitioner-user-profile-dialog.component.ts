import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProfileManagementService } from 'src/app/shared/services/profile.service';

export interface UserProfileDialogData {
  lastName: string;
  firstName: string;
  businessName: string;
  licensingInstitution: string;
  licenseNumber:string;
  emailAddress: string;
  
}
@Component({
  selector: 'app-edit-medical-practitioner-user-profile-dialog',
  templateUrl: './edit-medical-practitioner-user-profile-dialog.component.html',
  styleUrls: ['./edit-medical-practitioner-user-profile-dialog.component.scss'],
})
export class EditMedicalPractitionerUserProfileDialogComponent {
  @HostBinding('class') className = 'mat-dialog-container-host';

  constructor(
    public dialogRef: MatDialogRef<EditMedicalPractitionerUserProfileDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UserProfileDialogData,
    private profileService: ProfileManagementService
  ) {}

  onCancel() {
    this.dialogRef.close();
  }

  onConfirmChanges() {
    // get the updated data from form

  }
}
