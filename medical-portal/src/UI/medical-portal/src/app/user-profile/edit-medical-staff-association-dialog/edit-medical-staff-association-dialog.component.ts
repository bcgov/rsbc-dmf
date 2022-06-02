import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-edit-medical-staff-association-dialog',
  templateUrl: './edit-medical-staff-association-dialog.component.html',
  styleUrls: ['./edit-medical-staff-association-dialog.component.scss']
})
export class EditMedicalStaffAssociationDialogComponent {
  listOfPractitioners : string[] = ['Dr. Shelby', 'Dr. Robert'] 

  constructor() { }

  onConfirmChanges(){

  }

  onCancel(){

  }

}
