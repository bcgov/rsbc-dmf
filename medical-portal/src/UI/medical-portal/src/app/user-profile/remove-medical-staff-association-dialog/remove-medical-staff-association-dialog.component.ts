import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-remove-medical-staff-association-dialog',
  templateUrl: './remove-medical-staff-association-dialog.component.html',
  styleUrls: ['./remove-medical-staff-association-dialog.component.scss'],
})
export class RemoveMedicalStaffAssociationDialogComponent {
  @HostBinding('class') className = 'mat-dialog-container-host';
  displayedColumns: string[] = ['fullName', 'role', 'lastActive'];
  dataSource = this.data;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  onDeleteAssociation() {}

  onCancel() {}
}
