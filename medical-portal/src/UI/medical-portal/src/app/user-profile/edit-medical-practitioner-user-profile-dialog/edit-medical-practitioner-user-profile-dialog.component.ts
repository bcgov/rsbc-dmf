import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-edit-medical-practitioner-user-profile-dialog',
  templateUrl: './edit-medical-practitioner-user-profile-dialog.component.html',
  styleUrls: ['./edit-medical-practitioner-user-profile-dialog.component.scss'],
})
export class EditMedicalPractitionerUserProfileDialogComponent
{
  @HostBinding('class') className = 'mat-dialog-container-host';

  constructor() {}

  onConfirmChanges() {}

  onCancel() {}
}
