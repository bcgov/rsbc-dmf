import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { IgnitionInterlock, RehabTrigger } from '@app/shared/api/models';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserServices } from '@app/shared/services/user.service';

@Component({
  selector: 'app-rehab-interlock',
  standalone: true,
  imports: [CommonModule, DatePipe, MatCardModule, MatIconModule, MatExpansionModule],
  templateUrl: './rehab-interlock.component.html',
  styleUrl: './rehab-interlock.component.scss'
})
export class RehabInterlockComponent implements OnInit   {
 rehabInterlockDetails: RehabTrigger[] = [];
 ignitionInterlockDetails: IgnitionInterlock[] = [];
  isExpanded: Record<string, boolean> = {};

  constructor(
    private caseManagementService: CaseManagementService,
    private userService: UserServices) { }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  ngOnInit(): void {
    this.getRehabTriggerDetails();
    this.getIgnitionInterlockDetails();
  }

  getRehabTriggerDetails() {
    const driver = this.userService.getCachedriver();
    if (driver.id) {
      console.log('driver.id', driver.id);
      this.caseManagementService.getRehabTriggerDetails(driver.id as string)
        .subscribe({
          next: (rehabTriggerDetails) => {
            console.log('rehabInterlockDetails', rehabTriggerDetails);
            this.rehabInterlockDetails = rehabTriggerDetails;
          },
          error: (error) => {
            console.error('error', error);
            this.rehabInterlockDetails = [];
          }
        });
      }
    }

    getIgnitionInterlockDetails() {
      const driver = this.userService.getCachedriver(); 
      if (driver.id) {
        this.caseManagementService.getIgnitionInterlockDetails(driver.id as string)
          .subscribe({
            next: (ignitionInterlockDetails) => {
              console.log('ignitionInterlockDetails', ignitionInterlockDetails);
              // Handle the ignition interlock details as needed
              this.ignitionInterlockDetails = ignitionInterlockDetails;
            },      
            error: (error) => {
              console.error('error', error);
            } 
          });
      } 
    }
}
