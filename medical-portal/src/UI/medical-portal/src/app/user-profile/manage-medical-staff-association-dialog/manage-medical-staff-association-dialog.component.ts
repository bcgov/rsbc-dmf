import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-medical-staff-association-dialog',
  templateUrl: './manage-medical-staff-association-dialog.component.html',
  styleUrls: ['./manage-medical-staff-association-dialog.component.scss'],
})
export class ManageMedicalStaffAssociationDialogComponent {
  displayedColumns: string[] = ['fullName', 'role', 'lastActive'];
  dataSource = [
    {
      id: '1',
      fullName: 'Dr. Rajan Mehra',
      role: 'Medical Practitioner',
      lastActive: 'October 5, 2021',
    },
    // { id: "2", fullName: "Dr. Shelby Drew", role: "June 15, 2023", lastActive: "Active" },
  ];

  constructor() {}

  onManageAssociation() {}

  onCancel() {}
}
