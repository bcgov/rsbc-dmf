import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-manage-medical-practitioner-association-dialog',
  templateUrl:
    './manage-medical-practitioner-association-dialog.component.html',
  styleUrls: [
    './manage-medical-practitioner-association-dialog.component.scss',
  ],
})
export class ManageMedicalPractitionerAssociationDialogComponent {
  displayedColumns: string[] = ['fullName', 'role', 'lastActive'];
  dataSource = [this.data];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  onManageAssociation() {}

  onCancel() {}
}
