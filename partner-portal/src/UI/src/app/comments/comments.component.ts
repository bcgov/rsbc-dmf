import { DatePipe, NgFor, NgIf } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';
import { Comment } from '@app/shared/api/models';
import { CommentOrigin } from '@app/app.model';
@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [MatDialogContent, MatDialogActions, MatButtonModule, MatIconModule, MatCardModule, NgFor, NgIf, MatDialogClose, DatePipe],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
  // changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent implements OnInit {
  filterBy = CommentOrigin.User; 

  _allcommentRequest: Comment[] = [];
  // Get Driver details
  driverDetails = this.userService.getCachedriver();
 
   constructor(
    
    private caseManagementService: CaseManagementService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    if (this.driverDetails.id) {
      this.getComments(this.driverDetails.id as string)
    }
    else {
      console.log('No Comments for this user')
    }

  }


  @Input() set allComments(comments: Comment[]) {
    if (comments)
      this._allcommentRequest = comments;
    //this.filteredCallbacks = this._allCallBackRequests?.slice(0, this.pageSize);
  }

  get allComments() {
    return this._allcommentRequest;
  }

  getComments(driverId: string) {
    console.log(driverId)
    this.caseManagementService.getComments(driverId).subscribe((comments: any) => {
      this._allcommentRequest = comments;
    });
  }

  filterBySystem(){
   this.filterBy = CommentOrigin.User;
  }
 
  filterByUser(){
    this.filterBy = CommentOrigin.System; 
  }
}
