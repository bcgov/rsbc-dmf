import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-medical-practitioner-role-association-dialog',
  templateUrl:
    './create-medical-practitioner-role-association-dialog.component.html',
  styleUrls: [
    './create-medical-practitioner-role-association-dialog.component.scss',
  ],
})
export class CreateMedicalPractitionerRoleAssociationDialogComponent
{
  listOfPractitioners: string[] = ['Dr. Shelby', 'Dr. Robert'];
  @HostBinding('class') className = 'mat-dialog-container-host';

  constructor() {}

  onRequestAssociation() {}

  onCancel() {}
}
