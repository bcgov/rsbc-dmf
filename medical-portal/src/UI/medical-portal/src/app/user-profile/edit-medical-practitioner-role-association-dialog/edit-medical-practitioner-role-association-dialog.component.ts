import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';


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
  constructor(
    public dialogRef: MatDialogRef<EditMedicalPractitionerRoleAssociationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) 
    public data: any,
  ) {}

  onConfirmChanges() {}

  onCancel() {}
}
