import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';

@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule, MatCardModule],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent implements OnInit {

  _allcommentRequest?: Comment[] | null = [];

  constructor(private caseManagementService: CaseManagementService, private userService: UserService,) { }

  ngOnInit(): void {
    let userId = this.userService.getUserId();

    if (userId != null) {
      this.getComments(userId)
    }

  }

  getComments(driverLicenseNumber: string) {
    this.caseManagementService.getComments(driverLicenseNumber).subscribe((comments: any) => {
      this._allcommentRequest = comments;
    });
  }
}
