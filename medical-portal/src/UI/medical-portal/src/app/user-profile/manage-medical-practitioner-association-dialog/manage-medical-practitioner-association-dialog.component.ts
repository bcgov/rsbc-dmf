import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-medical-practitioner-association-dialog',
  templateUrl: './manage-medical-practitioner-association-dialog.component.html',
  styleUrls: ['./manage-medical-practitioner-association-dialog.component.scss']
})
export class ManageMedicalPractitionerAssociationDialogComponent{
  displayedColumns: string[] = ['fullName', 'role', 'lastActive'];
  dataSource = [
    { id: "1", fullName: "Dr. Rajan Mehra", role: "Medical Practioner", lastActive: "October 5, 2021" },
    // { id: "2", fullName: "Dr. Shelby Drew", role: "June 15, 2023", lastActive: "Active" },
  
  ]

  constructor() { }

 


  onManageAssociation(){

  }

  onCancel(){

  }
}
