import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RoleDescription } from '@app/features/auth/enums/identity-provider.enum';
import { AuthService } from '@app/features/auth/services/auth.service';
import { ProfileManagementService } from '@app/shared/services/profile.service';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [MatCardModule, MatIconModule, CommonModule, MatButtonModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss',
})
export class AccountComponent {
  isExpanded: Record<string, boolean> = {};
  fullName: string = "";
  email: string = "";
  role: string = "";

  public constructor(private authService: AuthService, private profileManagementService: ProfileManagementService)
  {
    this.profileManagementService.getProfile().subscribe((profile) => {
      this.fullName = profile.firstName + " " + profile.lastName;
      if (profile.email) {
        this.email = profile.email + "";
      }
      console.log("endorsement response", profile.endorsements);
    });
    this.role = this.authService
      .getRoles()
      .map((role) => RoleDescription.get(role))
      .join(", ");
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }
}
