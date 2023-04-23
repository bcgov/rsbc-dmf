import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-medical-staff-association-dialog',
  templateUrl: './create-medical-staff-association-dialog.component.html',
  styleUrls: ['./create-medical-staff-association-dialog.component.scss']
})
export class CreateMedicalStaffAssociationDialogComponent{
  @HostBinding('class') className = 'mat-dialog-container-host';
  listOfPractitioners : string[] = ['Dr. Shelby Drew', 'Will Mathews, NP'] 

  constructor() { }

  onRequestAssociation(){

  }

  onCancel(){

  }

}
