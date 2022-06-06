import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-edit-medical-practitioner-role-association-dialog',
  templateUrl:
    './edit-medical-practitioner-role-association-dialog.component.html',
  styleUrls: [
    './edit-medical-practitioner-role-association-dialog.component.scss',
  ],
})
export class EditMedicalPractitionerRoleAssociationDialogComponent {
  @HostBinding('class') className = 'mat-dialog-container-host';
  constructor() {}

  onConfirmChanges() {}

  onCancel() {}
}
