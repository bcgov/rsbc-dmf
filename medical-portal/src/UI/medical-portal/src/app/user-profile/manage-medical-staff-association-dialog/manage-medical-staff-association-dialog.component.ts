import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-manage-medical-staff-association-dialog',
  templateUrl: './manage-medical-staff-association-dialog.component.html',
  styleUrls: ['./manage-medical-staff-association-dialog.component.scss'],
})
export class ManageMedicalStaffAssociationDialogComponent {
  @HostBinding('class') className = 'mat-dialog-container-host';
  displayedColumns: string[] = ['moaName', 'medicalPractitionerName'];
  dataSource = [...this.data.selectedData];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  onManageAssociation() {}

  onCancel() {
    
  }
}
