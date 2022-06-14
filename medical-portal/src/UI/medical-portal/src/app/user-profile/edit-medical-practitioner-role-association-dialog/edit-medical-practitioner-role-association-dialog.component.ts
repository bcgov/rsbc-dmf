import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


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
