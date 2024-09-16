import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogActions, MatDialogClose, MatDialogContent } from '@angular/material/dialog';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-add-comments',
  standalone: true,
  imports: [MatDialogContent, MatDialogActions, MatDialogClose, MatIcon, MatInputModule, MatButtonModule],
  templateUrl: './add-comments.component.html',
  styleUrl: './add-comments.component.scss'
})
export class AddCommentsComponent {

}
