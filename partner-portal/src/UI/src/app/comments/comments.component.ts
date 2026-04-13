import { DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, Input, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MAT_DIALOG_DATA, MatDialogClose, MatDialogContent, MatDialogRef } from '@angular/material/dialog';
import { MatError, MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TextFieldModule } from '@angular/cdk/text-field';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';
import { Comment } from '@app/shared/api/models';
import { CommentOrigin } from '@app/app.model';
import { MatTooltipModule} from '@angular/material/tooltip';
import { of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [
    MatDialogContent, 
    MatButtonModule, 
    MatIconModule, 
    MatCardModule, 
    NgFor, 
    NgIf, 
    MatDialogClose, 
    DatePipe, 
    MatTooltipModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatError,
    TextFieldModule,
  ],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
  // changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent implements OnInit {
  CommentOrigin = CommentOrigin;

  filterBy: CommentOrigin | null = null; 
  isAddingComment = false;
  isLoadingComments = false;
  isSavingComment = false;

  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  visibleCount = this.pageSize;
  filteredComments : Comment[] = [];
  filteredCommentsCount = 0;
  _allcommentRequest: Comment[] = [];
  currentDriverId: string | null = null;
  commentsForm = this.fb.group({
    commentText: ['', Validators.compose([Validators.required, Validators.maxLength(2000)]),],
  });
 
   constructor(
    private dialogRef: MatDialogRef<CommentsComponent>,
    private caseManagementService: CaseManagementService,
    private userService: UserService,
    private fb: FormBuilder
  ) { }

  readonly dialogDriverId = inject(MAT_DIALOG_DATA, { optional: true }) as string | null;

  ngOnInit(): void {
    const cachedDriverId = this.userService.getCachedriver()?.id ?? null;
    this.currentDriverId = this.dialogDriverId || cachedDriverId;

    if (this.currentDriverId) {
      this.getComments(this.currentDriverId);
    } else {
      console.log('No Comments for this user')
    }

  }

  @Input() set allComments(comments: Comment[]) {
    if (comments) {
      this._allcommentRequest = comments;
    }

    this.visibleCount = this.pageSize;
    this.applyFilter();
  }

  get allComments() {
    return this._allcommentRequest;
  }

  get allCommentsLength() {
    return this.filteredCommentsCount;
  }

  getComments(driverId: string) {
    if (!driverId) {
      this._allcommentRequest = [];
      this.applyFilter();
      return;
    }

    this.isLoadingComments = true;
    this.caseManagementService.getComments(driverId).pipe(
      catchError((error) => {
        console.error('Unable to load comments:', error);
        return of([] as Comment[]);
      }),
      finalize(() => {
        this.isLoadingComments = false;
      })
    ).subscribe((comments: Comment[]) => {
      this._allcommentRequest = comments ?? [];
      this.visibleCount = this.pageSize;
      this.applyFilter();
    });
  }

  filterByAllComments(){
    this.filterBy = null;
    this.visibleCount = this.pageSize;
    this.applyFilter();
   }

  filterByUser(){
   this.filterBy = CommentOrigin.User;
   this.visibleCount = this.pageSize;
   this.applyFilter();
  }
 
  filterBySystem(){
    this.filterBy = CommentOrigin.System; 
    this.visibleCount = this.pageSize;
    this.applyFilter();
  }
  
  addComment(){
    this.isAddingComment = !this.isAddingComment;
    if (!this.isAddingComment) {
      this.commentsForm.reset();
    }
  }

  saveComment() {
    if (this.isSavingComment) {
      return;
    }

    const driverId = this.currentDriverId;
    if (!driverId) {
      console.error('Unable to add comment: missing driver id.');
      return;
    }

    const trimmedCommentText = this.commentsForm.controls.commentText.value?.trim() ?? '';
    this.commentsForm.controls.commentText.setValue(trimmedCommentText);

    if (this.commentsForm.invalid) {
      this.commentsForm.markAllAsTouched();
      return;
    }

    const comment = {
      commentText: trimmedCommentText,
    };

    this.isSavingComment = true;
    this.caseManagementService.addComments({ body: comment }).pipe(
      finalize(() => {
        this.isSavingComment = false;
      })
    ).subscribe({
      next: () => {
        this.commentsForm.reset();
        this.isAddingComment = false;
        this.getComments(driverId);
      },
      error: (error) => {
        console.error('Unable to add comment:', error);
      }
    });
  }

  viewMore() {
    this.visibleCount = this.filteredComments.length + this.pageSize;
    this.applyFilter();
  }

  trackByCommentId(index: number, comment: Comment) {
    return comment.commentId ?? index;
  }

  private applyFilter() {
    const source = this._allcommentRequest ?? [];
    const filtered = this.filterBy
      ? source.filter((c) => c.origin === this.filterBy)
      : source;

    this.filteredCommentsCount = filtered.length;
    this.filteredComments = filtered.slice(0, this.visibleCount);
  }
  
  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  expandLarge(){
    this.dialogRef?.updateSize('1361px','730px' ); 
  }

  expandMedium(){
    this.dialogRef?.updateSize('620px','730px'); 
  }

  expandSmall(){
    this.dialogRef?.updateSize('400px', '730px'); 
  }
}
