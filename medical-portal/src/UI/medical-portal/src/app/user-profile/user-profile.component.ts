import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UserProfile } from '../shared/api/models';
import { ProfileManagementService } from '../shared/services/profile.service';
import { CreateMedicalPractitionerAssociationDialogComponent } from './create-medical-practitioner-association-dialog/create-medical-practitioner-association-dialog.component';
import { EditUserProfileDialogComponent } from './edit-user-profile-dialog/edit-user-profile-dialog.component';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
 userProfile!: UserProfile;
 firstName!: string;
 lastName!:string;
 emailAddress!: string;
 
 statuses = [
  { label: 'Select Action' },
  { label: 'Accept Selected' },
  { label: 'Request Renewal' },
  { label: 'Reject Selected' },
  { label: 'Deactivate Selected' },
  { label: 'Remove Selected' },
];

dataSource = [
  { id: "1", fullName: "Dr. Rajan Mehra", expiryDate: "May 10, 2023", status: "pending" },
  { id: "2", fullName: "Dr. Shelby Drew", expiryDate: "June 15, 2023", status: "Active" },
  { id: "3", fullName: "Devi Iyer, NP", expiryDate: "Sep 30, 2024", status: "Active" },
  { id: "4", fullName: "Dr. Tarik Haiga", expiryDate: "July 9, 2025", status: "InActive" },
];

  constructor(public dialog: MatDialog, public profileService: ProfileManagementService) {}
  ngOnInit(): void {
    this.profileService.getProfile({}).subscribe((profile)=> 
     this.userProfile = profile
    );
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(EditUserProfileDialogComponent, {
      height: '600px',
      width: '820px',
      data: {firstName: this.userProfile.firstName, lastName: this.userProfile.lastName, emailAddress: this.userProfile.emailAddress },
      
    });

    

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed', result);
      this.userProfile.emailAddress = result;

    });
  }

  openCreateMedicalPractitionerAssociationDialog(){
    const dialogRef = this.dialog.open(CreateMedicalPractitionerAssociationDialogComponent, {
      height: '600px',
      width: '820px',
      //data: {firstName: this.userProfile.firstName, lastName: this.userProfile.lastName, emailAddress: this.userProfile.emailAddress },
      
    });

    

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed', result);
      //this.userProfile.emailAddress = result;

    });
  }

  searchMedicalPractitioners(){

  }

  clear(){

  }

}

