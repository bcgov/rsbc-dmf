import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Sort } from '@angular/material/sort';
import { UserProfile } from '../shared/api/models';
import { ProfileManagementService } from '../shared/services/profile.service';
import { CreateMedicalPractitionerAssociationDialogComponent } from './create-medical-practitioner-association-dialog/create-medical-practitioner-association-dialog.component';
import { CreateMedicalStaffAssociationDialogComponent } from './create-medical-staff-association-dialog/create-medical-staff-association-dialog.component';
import { EditMedicalStaffAssociationDialogComponent } from './edit-medical-staff-association-dialog/edit-medical-staff-association-dialog.component';
import { EditUserProfileDialogComponent } from './edit-user-profile-dialog/edit-user-profile-dialog.component';
import { ManageMedicalPractitionerAssociationDialogComponent } from './manage-medical-practitioner-association-dialog/manage-medical-practitioner-association-dialog.component';
import { ManageMedicalStaffAssociationDialogComponent } from './manage-medical-staff-association-dialog/manage-medical-staff-association-dialog.component';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent implements OnInit {
  userProfile!: UserProfile;
  firstName!: string;
  lastName!: string;
  emailAddress!: string;
  public selectedPractitionerStatus: string = 'Select Action';
  displayedColumns: string[] = [
    'select',
    'fullName',
    'expiryDate',
    'status',
    'action',
  ];
  displayedMedicalStaffColumns: string[] = [
    'select',
    'fullName',
    'medicalPractitionerName',
    'expiryDate',
    'status',
    'editAction',
    'removeAction',
  ];

  practitionerSelection = new SelectionModel<any>(true, []);

  MOAstatuses = [
    { label: 'Select Action' },
    { label: 'Request Renewal' },
    { label: 'Deactivate Selected' },
    { label: 'Remove Selected' },
  ];

  MOMstatuses = [
    { label: 'Select Action' },
    { label: 'Accept Selected' },
    { label: 'Request Renewal' },
    { label: 'Reject Selected' },
    { label: 'Deactivate Selected' },
    { label: 'Remove Selected' },
  ];

  dataSource = [
    {
      id: '1',
      fullName: 'Dr. Rajan Mehra',
      expiryDate: 'May 10, 2022',
      status: 'pending',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '2',
      fullName: 'Dr. Shelby Drew',
      expiryDate: 'June 15, 2023',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '3',
      fullName: 'Devi Iyer, NP',
      expiryDate: 'March 30, 2024',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '4',
      fullName: 'Dr. Tarik Haiga',
      expiryDate: 'July 9, 2025',
      status: 'InActive',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
  ];

  medicalStaffDataSource = [
    {
      id: '1',
      fullName: 'Torres, Sharon',
      medicalPractitionerName: 'Dr. Rajan Mehra',
      expiryDate: 'May 10, 2022',
      status: 'pending',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '2',
      fullName: 'Smith, Ingrid',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: 'June 15, 2023',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '3',
      fullName: 'Tucker, Devi',
      medicalPractitionerName: 'Devi Iyer, NP',
      expiryDate: 'March 30, 2024',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '4',
      fullName: 'Varga, Tarik',
      medicalPractitionerName: 'Dr. Tarik Haiga',
      expiryDate: 'July 9, 2025',
      status: 'InActive',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
  ];

  constructor(
    public dialog: MatDialog,
    public profileService: ProfileManagementService
  ) {}
  ngOnInit(): void {
    this.profileService
      .getProfile({})
      .subscribe((profile) => (this.userProfile = profile));
  }

  // User Profile
  openEditUserProfileDialog(): void {
    const dialogRef = this.dialog.open(EditUserProfileDialogComponent, {
      height: '600px',
      width: '820px',
      data: {
        firstName: this.userProfile.firstName,
        lastName: this.userProfile.lastName,
        emailAddress: this.userProfile.emailAddress,
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      this.userProfile.emailAddress = result;
    });
  }

  // Medical Staff Association

  openCreateMedicalStaffAssociationDialog() {
    const dialogRef = this.dialog.open(
      CreateMedicalStaffAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        //data: {firstName: this.userProfile.firstName, lastName: this.userProfile.lastName, emailAddress: this.userProfile.emailAddress },
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;
    });
  }

  openEditMedicalStaffAssociationDialog(row: any) {
    const dialogRef = this.dialog.open(
      EditMedicalStaffAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: row,
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;
    });
  }

  openManageMedicalStaffAssociationDialog(row: any) {
    const dialogRef = this.dialog.open(
      ManageMedicalStaffAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: row,
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;
    });
  }

  // Medical Practitioner Association

  openCreateMedicalPractitionerAssociationDialog() {
    const dialogRef = this.dialog.open(
      CreateMedicalPractitionerAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        //data: {firstName: this.userProfile.firstName, lastName: this.userProfile.lastName, emailAddress: this.userProfile.emailAddress },
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;
    });
  }

  openManageMedicalPractitionerAssociationDialog(row: any) {
    const dialogRef = this.dialog.open(
      ManageMedicalPractitionerAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: {
          selectedStatus :"Remove Selected",
          selectedData:[row]
        },
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;
    });
  }

  searchMedicalPractitioners() {}

  medicalPractitionerStatusChange() {
    if (this.practitionerSelection.selected.length == 0) {
      // @TODO: Not selected any rows so show error message
      return;
    }

    const selectedStatus = this.selectedPractitionerStatus;

   const selectedData = this.practitionerSelection.selected;
    console.log(selectedData);

    this.dialog.open(ManageMedicalPractitionerAssociationDialogComponent, {
      data: {
        selectedStatus,
        selectedData: [...selectedData],
        
      }
    })
  }

  clear() {}

  sortMedicalStaffData(sort: Sort) {
    const data = this.medicalStaffDataSource.slice();
    if (!sort.active || sort.direction === '') {
      this.medicalStaffDataSource = data;
      return;
    }

    this.medicalStaffDataSource = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'fullName':
          return compare(a.fullName, b.fullName, isAsc);
        case 'medicalPractitionerName':
          return compare(
            a.medicalPractitionerName,
            b.medicalPractitionerName,
            isAsc
          );
        case 'expiryDate':
          return compare(a.expiryDate, b.expiryDate, isAsc);
        case 'status':
          return compare(a.status, b.status, isAsc);
        default:
          return 0;
      }
    });
  }

  sortMedicalPractitionerData(sort: Sort) {
    const data = this.dataSource.slice();
    if (!sort.active || sort.direction === '') {
      this.dataSource = data;
      return;
    }

    this.dataSource = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'fullName':
          return compare(a.fullName, b.fullName, isAsc);
        case 'expiryDate':
          return compare(a.expiryDate, b.expiryDate, isAsc);
        case 'status':
          return compare(a.status, b.status, isAsc);
        default:
          return 0;
      }
    });
  }
}

function compare(
  a: string | undefined | null,
  b: number | string | undefined | null,
  isAsc: boolean
) {
  // check for null or undefined
  if (a == null || b == null) {
    return 1;
  }
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
