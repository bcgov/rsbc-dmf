import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-manage-medical-practitioner-role-association-dialog',
  templateUrl: './manage-medical-practitioner-role-association-dialog.component.html',
  styleUrls: ['./manage-medical-practitioner-role-association-dialog.component.scss']
})
export class ManageMedicalPractitionerRoleAssociationDialogComponent {

  @HostBinding('class') className = 'mat-dialog-container-host';
  displayedColumns: string[] = ['fullName', 'role','lastActive'];
  dataSource = [...this.data.selectedData];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
   onManageAssociation() {}

  onCancel() {
    
  }



}
