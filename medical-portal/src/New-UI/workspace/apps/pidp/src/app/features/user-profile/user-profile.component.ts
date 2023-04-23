import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Sort } from '@angular/material/sort';
import { map } from 'rxjs';
import { ProviderRole, UserProfile } from '../shared/api/models';
import { ProfileManagementService } from '../shared/services/profile.service';
import { CreateMedicalPractitionerAssociationDialogComponent } from './create-medical-practitioner-association-dialog/create-medical-practitioner-association-dialog.component';
import { CreateMedicalPractitionerRoleAssociationDialogComponent } from './create-medical-practitioner-role-association-dialog/create-medical-practitioner-role-association-dialog.component';
import { CreateMedicalStaffAssociationDialogComponent } from './create-medical-staff-association-dialog/create-medical-staff-association-dialog.component';
import { EditMedicalPractitionerRoleAssociationDialogComponent } from './edit-medical-practitioner-role-association-dialog/edit-medical-practitioner-role-association-dialog.component';
import { EditMedicalPractitionerUserProfileDialogComponent } from './edit-medical-practitioner-user-profile-dialog/edit-medical-practitioner-user-profile-dialog.component';
import { EditMedicalStaffAssociationDialogComponent } from './edit-medical-staff-association-dialog/edit-medical-staff-association-dialog.component';
import { EditUserProfileDialogComponent } from './edit-user-profile-dialog/edit-user-profile-dialog.component';
import { ManageMedicalPractitionerAssociationDialogComponent } from './manage-medical-practitioner-association-dialog/manage-medical-practitioner-association-dialog.component';
import { ManageMedicalPractitionerRoleAssociationDialogComponent } from './manage-medical-practitioner-role-association-dialog/manage-medical-practitioner-role-association-dialog.component';
import { ManageMedicalStaffAssociationDialogComponent } from './manage-medical-staff-association-dialog/manage-medical-staff-association-dialog.component';
import { RemoveMedicalStaffAssociationDialogComponent } from './remove-medical-staff-association-dialog/remove-medical-staff-association-dialog.component';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent implements OnInit {
  allRoles = ProviderRole;
  userProfile!: UserProfile;
  firstName!: string;
  lastName!: string;
  emailAddress!: string;
  selectedPractitionerStatus: string = 'Select Action';
  selectedStaffStatus: string = 'Select Action';
  selectedMedicalPractitionerStatus = 'Select Action';
  selectedUserRole: string = 'All Roles';

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
    'removeAction',
  ];

  displayedMedicalPractitionerProfileColumns: string[] = [
    'select',
    'fullName',
    'role',
    'expiryDate',
    'status',
    'editAction',
    'removeAction',
  ];

  practitionerSelection = new SelectionModel<any>(true, []);
  staffSelection = new SelectionModel<any>(true, []);
  medicalPractitionerSelection = new SelectionModel<any>(true, []);

  MOAstatuses = [
    { label: 'Select Action' },
    { label: 'Request Renewal' },
    { label: 'Deactivate Selected' },
    { label: 'Remove Selected' },
  ];

  MOMstatuses = [
    { label: 'Select Action' },
    { label: 'Accept Selected' },
    { label: 'Renew Selected' },
    { label: 'Reject Selected' },
    { label: 'Deactivate Selected' },
    { label: 'Remove Selected' },
  ];

  MedicalPractitionerStatuses = [
    { label: 'Select Action' },
    { label: 'Accept Selected' },
    { label: 'Renew Selected' },
    { label: 'Reject Selected' },
    { label: 'Deactivate Selected' },
    { label: 'Remove Selected' },
  ];

  rolesFilter = [
    { label: 'All Roles' },
    { label: 'Medical Office Assistant' },
    { label: 'Medical Office Manager' },
  ];

  MedicalStaffFilter = [
    { label: 'All Medical Practitioners' },
    { label: 'Dr. Shelby Drew' },
    { label: 'Will Mathews, NP' },
  ];


  dataSource = [
    {
      id: '1',
      fullName: 'Dr. Shelby Drew',
      expiryDate: 'July 5, 2022',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '2',
      fullName: 'Dr. Sally Jones',
      expiryDate: '-',
      status: 'Rejected',
      role: 'Medical Practitioner',
      lastActive: 'September 11, 2021',
    },
    {
      id: '3',
      fullName: 'Mark Watkins, NP',
      expiryDate: 'March 2, 2022',
      status: 'Inactive',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '4',
      fullName: 'Eric Vern, NP',
      expiryDate: '-',
      status: 'Pending',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '5',
      fullName: 'Will Mathews, NP',
      expiryDate: 'October 10, 2022',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
  ];

  medicalStaffDataSource = [
    {
      id: '1',
      fullName: 'Torres, Sharon',
      medicalPractitionerName: 'Will Mathews, NP',
      expiryDate: 'August 24, 2022',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '2',
      fullName: 'Tucker, Devi',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: '-',
      status: 'Pending',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '3',
      fullName: 'Varga, Tarik',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: 'February 1, 2022',
      status: 'Inactive',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    
    {
      id: '4',
      fullName: 'Marsh, Caleb',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: '-',
      status: 'Pending',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '5',
      fullName: 'Torres, Sharon',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: 'July 5, 2022',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '6',
      fullName: 'Dodge, Mike',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: 'September 17, 2022',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },

    {
      id: '7',
      fullName: 'Mehra, Rajan',
      medicalPractitionerName: 'Tobi McIntosh, NP',
      expiryDate: '-',
      status: 'Rejected',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '8',
      fullName: 'Ram, Peter ',
      medicalPractitionerName: 'Will Mathews, NP',
      expiryDate: '-',
      status: 'Pending',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '9',
      fullName: 'Smith, John',
      medicalPractitionerName: 'Will Mathews, NP',
      expiryDate: '-',
      status: 'Rejected',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '10',
      fullName: 'Varga, Tarik',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: '-',
      status: 'Rejected',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },
    {
      id: '11',
      fullName: 'Castor, Ingrid',
      medicalPractitionerName: 'Dr. Shelby Drew',
      expiryDate: 'July 5, 2022',
      status: 'Active',
      role: 'Medical Practitioner',
      lastActive: 'May 11, 2022',
    },

  ];

  medicalPractitionerProfileDataSource = [
    {
      id: '1',
      fullName: 'Castor, Ingrid',
      firstName:'Ingrid',
      lastName:'Castor',
      role: 'MOA',
      expiryDate: 'July 5, 2022',
      status: 'Active',
      lastActive: 'October 5, 2021',
      email:'Ingrid@gmail.com'
    },
    {
      id: '2',
      fullName: 'Dodge, Mike',
      firstName:'Mike',
      lastName:'Dodge',
      role: 'MOA',
      expiryDate: 'September 17, 2022',
      status: 'Active',
      lastActive: 'March 11, 2021',
      email:'Mike@gmail.com'
    },
    {
      id: '3',
      fullName: 'Mehra, Rajan',
      firstName:'Rajan',
      lastName:'Mehra',
      role: 'MOM',
      expiryDate: '-',
      status: 'Inactive',
      lastActive: 'May 11, 2022',
      email:'Rajan@gmail.com'
    },
    {
      id: '4',
      fullName: 'Varga, Tarik',
      firstName:'Tarik',
      lastName:'Varga',
      role: 'MOA',
      expiryDate: 'February 1, 2022',
      status: 'Inactive',
      lastActive: 'May 11, 2022',
      email:'Tarik@gmail.com'
    },
    {
      id: '5',
      fullName: 'Tucker, Devi',
      role: 'MOA',
      firstName:'Tucker',
      lastName:'Varga',
      expiryDate: '-',
      status: 'Pending',
      lastActive: 'May 11, 2022',
      email:'Devi@gmail.com'
    },
    {
      id: '6',
      fullName: 'Marsh, Caleb',
      firstName:'Caleb',
      lastName:'Marsh',
      role: 'MOA',
      expiryDate: '-',
      status: 'Pending',
      lastActive: 'May 11, 2022',
      email:'Caleb@gmail.com'
    },
    {
      id: '7',
      fullName: 'Torres, Sharon',
      firstName:'Sharon',
      lastName:'Torres',
      role: 'MOA',
      expiryDate: 'July 5, 2022',
      status: 'Active',
      lastActive: 'March 11, 2022',
      email:'Sharon@gmail.com'
    },
    {
      id: '8',
      fullName: 'Varga, Tarik',
      firstName:'Tarik',
      lastName:'Varga',
      role: 'MOA',
      expiryDate:'',
      status: 'Rejected',
      lastActive: 'May 11, 2022',
      email:'Tarik@gmail.com'
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

    // Get data to Medical Practitioner Roles Table

    this.profileService.getProfilePractitionerRoles({}).subscribe((profile) => {
      console.log('Get Practioners roles data', profile);
    });

    // Sort Medical Staff table
    this.sortMedicalPractitionerRolesData({active:'fullName', direction: 'asc'});

    // Sort Medical Staff table
    this.sortMedicalStaffData({active:'fullName', direction: 'asc'});

    // Sort Medical Practitioner table
    this.sortMedicalPractitionerData({active:'fullName', direction: 'asc'});


  }

  permissions(
    feature:
      | 'manageUserProfile'
      | 'manageMedicalPractitionerUserProfile'
      | 'manageMedicalPractitionerAssociations'
      | 'manageMedicalPractitionerAssociationTable'
      | 'manageMedicalStaffAssociation'
      | 'manageMedicalStaffTable'
      | 'manageMOAAssociation'
      | 'manageMOMAssociation'
      | 'manageMOASearch'
      | 'manageMOATable'
  ) {
    switch (feature) {
      case 'manageUserProfile':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) =>
                UserRole.role === ProviderRole.MedicalOfficeAssistant
            ) ||
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalOfficeManager
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMedicalPractitionerUserProfile':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalPractitioner
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;

      case 'manageMedicalPractitionerAssociations':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalPractitioner
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMedicalPractitionerAssociationTable':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalPractitioner
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;

      case 'manageMedicalStaffAssociation':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalOfficeManager
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMedicalStaffTable':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalOfficeManager
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMOAAssociation':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) =>
                UserRole.role === ProviderRole.MedicalOfficeAssistant
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMOMAssociation':
        {
          if (
            this.userProfile?.clinics?.some((UserRole) => {
              return UserRole.role === ProviderRole.MedicalOfficeManager;
            })
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMOASearch':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) =>
                UserRole.role === ProviderRole.MedicalOfficeAssistant
            ) ||
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalOfficeManager
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;
      case 'manageMOATable':
        {
          if (
            this.userProfile?.clinics?.some(
              (UserRole) =>
                UserRole.role === ProviderRole.MedicalOfficeAssistant
            ) ||
            this.userProfile?.clinics?.some(
              (UserRole) => UserRole.role === ProviderRole.MedicalOfficeManager
            )
          ) {
            return true;
          } else {
            return false;
          }
        }
        break;

      default:
        return false;
    }
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
      if(!result) return
      this.userProfile.emailAddress = result;
    });
  }

  // Medical Practitioner User Profile

  openEditMedicalPractitionerUserProfileDialogComponent(): void {
    const dialogRef = this.dialog.open(
      EditMedicalPractitionerUserProfileDialogComponent,
      {
        height: '650px',
        width: '820px',
        data: {
          ...this.userProfile,
        },
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
    });
  }

  openCreateMedicalPractitionerRoleAssociationDialog(): void {
    const dialogRef = this.dialog.open(
      CreateMedicalPractitionerRoleAssociationDialogComponent,
      {
        height: '620px',
        width: '820px',
        // data
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      this.userProfile.emailAddress = result;
    });
  }

  openEditMedicalPractitionerRoleAssociationDialog(row:any): void {
    const dialogRef = this.dialog.open(
      EditMedicalPractitionerRoleAssociationDialogComponent,
      {
        height: '720px',
        width: '820px',
        data: row,
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      if(!result) return
      this.userProfile.emailAddress = result;
    });
  }

  openManageMedicalPractitionerRoleAssociationDialog(row: any): void {
    const dialogRef = this.dialog.open(
      ManageMedicalPractitionerRoleAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: {
          selectedMedicalPractitionerStatus: 'Remove Selected',
          selectedData: [row],
        },
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
     if(!result) return
      this.userProfile.emailAddress = result;
    });
  }

  medicalPractitionerProfileStatusChange() {
    if (this.medicalPractitionerSelection.selected.length == 0) {
      // @TODO: Not selected any rows so show error message
      return;
    }

    const selectedMedicalPractitionerStatus =
      this.selectedMedicalPractitionerStatus;

    const selectedData = this.medicalPractitionerSelection.selected;
    console.log(selectedData);

    const dialogRef = this.dialog.open(
      ManageMedicalPractitionerRoleAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: {
          selectedMedicalPractitionerStatus,
          selectedData: [...selectedData],
        },
      }
    );

    dialogRef.afterClosed().subscribe((selectedAction) => {
      this.medicalPractitionerSelection.clear();
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

  openRemoveMedicalStaffAssociationDialog(row: any) {
    const dialogRef = this.dialog.open(
      RemoveMedicalStaffAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: [row],
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
        data: {
          selectedStaffStatus: 'Remove Selected',
          selectedData: [row],
        },
      }
    );
    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;
    });
  }

  medicalStaffStatusChange() {
    if (this.staffSelection.selected.length == 0) {
      // @TODO: Not selected any rows so show error message
      return;
    }

    const selectedStaffStatus = this.selectedStaffStatus;

    const selectedData = this.staffSelection.selected;
    console.log(selectedData);

    const dialogRef = this.dialog.open(
      ManageMedicalStaffAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: {
          selectedStaffStatus,
          selectedData: [...selectedData],
        },
      }
    );
    dialogRef.afterClosed().subscribe((selectedAction) => {
      this.staffSelection.clear();
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
          selectedStatus: 'Remove Selected',
          selectedData: [row],
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

    const dialogRef = this.dialog.open(
      ManageMedicalPractitionerAssociationDialogComponent,
      {
        height: '600px',
        width: '820px',
        data: {
          selectedStatus,
          selectedData: [...selectedData],
        },
      }
    );

    dialogRef.afterClosed().subscribe((selectedAction) => {
      this.practitionerSelection.clear();
    });
  }

  clear() {}

  sortMedicalPractitionerRolesData(sort: Sort) {
    const data = this.medicalPractitionerProfileDataSource.slice();
    if (!sort.active || sort.direction === '') {
      this.medicalPractitionerProfileDataSource = data;
      return;
    }

    this.medicalPractitionerProfileDataSource = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'fullName':
          return compare(a.fullName, b.fullName, isAsc);
        case 'role':
          return compare(a.role, b.role, isAsc);
        case 'expiryDate':
          return compare(a.expiryDate, b.expiryDate, isAsc);
        case 'status':
          return compare(a.status, b.status, isAsc);
        default:
          return 0;
      }
    });
  }

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
    console.log(sort)
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

  translatePortalUserRole(role: any) {
    switch (role) {
      case 1:
        return 'Medical Practitioner';
      case 2:
        return 'Medical Office Manager';
      case 3:
        return 'Medical Office Assistant';
      default:
        return 'None';
    }
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
