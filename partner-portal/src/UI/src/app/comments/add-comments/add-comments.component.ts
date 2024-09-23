import { Component } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef } from '@angular/material/dialog';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';

@Component({
  selector: 'app-add-comments',
  standalone: true,
  imports: [MatDialogContent, MatDialogActions, MatDialogClose, MatIcon, MatInputModule, MatButtonModule,MatError, FormsModule, ReactiveFormsModule],
  templateUrl: './add-comments.component.html',
  styleUrl: './add-comments.component.scss'
})
export class AddCommentsComponent {

  constructor(
    private caseManagementService: CaseManagementService,
    private fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<AddCommentsComponent>
  ) { }

  commentsForm = this.fb.group({
    commentText : ['', Validators.required]
  })

  AddComment(){   
    const comment: any = {
      commentText : this.commentsForm.value.commentText
    }
     this.caseManagementService.addComments({body: comment}).subscribe(() => {
       this.commentsForm.reset();
       this._snackBar.open('Successfully created Comment', 'Close', {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 5000,
      });
      this.dialogRef.close();
     })
  }

}
