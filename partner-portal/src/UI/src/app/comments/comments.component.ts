import { DatePipe, NgFor, NgIf } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit, Inject, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MAT_DIALOG_DATA, MatDialog, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';
import { Comment } from '@app/shared/api/models';
import { CommentOrigin } from '@app/app.model';
import { AddCommentsComponent } from './add-comments/add-comments.component';
@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [
    MatDialogContent, 
    MatDialogActions, 
    MatButtonModule, 
    MatIconModule, 
    MatCardModule, 
    NgFor, 
    NgIf, 
    MatDialogClose, 
    DatePipe, 
    AddCommentsComponent
  ],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
  // changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent implements OnInit {
  CommentOrigin = CommentOrigin;
  readonly dialog = inject(MatDialog);

  filterBy: CommentOrigin | null = null; 

  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  filteredComments : Comment[] | null = [];
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
      this.filteredComments = this._allcommentRequest.slice(0, this.pageSize);
  }

  get allComments() {
    return this._allcommentRequest;
  }

  get allCommentsLength() {
    return this.allComments.filter((c) => !this.filterBy || c.origin === this.filterBy).length;
  }

  getComments(driverId: string) {
    this.caseManagementService.getComments(driverId).subscribe((comments: any) => {
      this._allcommentRequest = comments;
      this.filteredComments = this._allcommentRequest?.slice(0, this.pageSize);
    });
  }

  filterByAllComments(){
    this.filterBy = null;
    this.filteredComments = this._allcommentRequest?.filter((c) => !this.filterBy || c.origin === this.filterBy).slice(0, this.pageSize);
   }

  filterByUser(){
   this.filterBy = CommentOrigin.User;
   this.filteredComments = this._allcommentRequest?.filter((c) => !this.filterBy || c.origin === this.filterBy).slice(0, this.pageSize);
  }
 
  filterBySystem(){
    this.filterBy = CommentOrigin.System; 
    this.filteredComments = this._allcommentRequest?.filter((c) => !this.filterBy || c.origin === this.filterBy).slice(0, this.pageSize);
  }

  addComment(){
    const dialogRef = this.dialog.open(AddCommentsComponent, {
      height: '300px',
      width: '275px',
      position: {
        bottom: '200px',
        right: '60px',
      },
    });
    dialogRef.afterClosed()  
    .subscribe({
      next:() => {
      let driverId = this.driverDetails.id;
      if(driverId != null){
        this.getComments(driverId);
      }   
      }   
    });

  }

  viewMore() {
    const pageSize = (this.filteredComments?.length ?? 0) + this.pageSize;

    this.filteredComments = this._allcommentRequest?.filter((c) => !this.filterBy || c.origin === this.filterBy).slice(0, pageSize);
  }
  
  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

}
