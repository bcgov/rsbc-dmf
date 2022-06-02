import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-medical-staff-association-dialog',
  templateUrl: './create-medical-staff-association-dialog.component.html',
  styleUrls: ['./create-medical-staff-association-dialog.component.scss']
})
export class CreateMedicalStaffAssociationDialogComponent{
  listOfPractitioners : string[] = ['Dr. Shelby', 'Dr. Robert'] 

  constructor() { }

  onRequestAssociation(){

  }

  onCancel(){

  }

}
