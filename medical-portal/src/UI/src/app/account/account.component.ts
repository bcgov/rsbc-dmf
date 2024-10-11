import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Role, RoleDescription } from '@app/features/auth/enums/role.enum';
import { RoleService } from '@app/features/auth/services/role.service';
import { Endorsement } from '@app/shared/api/models';
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
  endorsements: Endorsement[] = [];

  public constructor(private roleService: RoleService, private profileManagementService: ProfileManagementService)
  {
    this.profileManagementService.getProfile().subscribe((profile) => {
      console.info('profile response', profile);
      this.fullName = profile.firstName + " " + profile.lastName;
      if (profile.email) {
        this.email = profile.email + "";
      }
      if (profile.endorsements) {
        this.endorsements = profile.endorsements;
      }
    });
    this.role = this.roleService
      .getRoles()
      .map((role: Role) => RoleDescription.get(role))
      .join(", ");
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }
}
