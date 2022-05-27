import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UserProfile } from '../shared/api/models';
import { ProfileManagementService } from '../shared/services/profile.service';
import { EditUserProfileDialogComponent } from './edit-user-profile-dialog/edit-user-profile-dialog.component';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
 userProfile!: UserProfile;
 

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
      // data: {name: this.name, animal: this.animal},
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      
    });
  }

}

