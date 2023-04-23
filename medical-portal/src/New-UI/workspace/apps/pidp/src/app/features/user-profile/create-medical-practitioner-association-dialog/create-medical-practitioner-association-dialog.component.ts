import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-medical-practitioner-association-dialog',
  templateUrl:
    './create-medical-practitioner-association-dialog.component.html',
  styleUrls: [
    './create-medical-practitioner-association-dialog.component.scss',
  ],
})
export class CreateMedicalPractitionerAssociationDialogComponent {
  @HostBinding('class') className = 'mat-dialog-container-host';

  constructor() {}

  onRequestAssociation() {}

  onCancel() {}
}
