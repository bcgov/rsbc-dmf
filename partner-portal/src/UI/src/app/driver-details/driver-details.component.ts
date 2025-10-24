import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { UserContext } from '@app/shared/api/models';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';

@Component({
  selector: 'app-driver-details',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: './driver-details.component.html',
  styleUrl: './driver-details.component.scss'
})
export class DriverDetailsComponent {
  cachedDriver = this.userService.getCachedriver();
  driverDetails: UserContext = this.cachedDriver;

  constructor(private userService: UserService, private caseManagementService: CaseManagementService) {  }
  
  ngOnInit() {
    this.getDriverDetails();
  }

  getDriverDetails() {
    this.caseManagementService
      .searchByDriver({ driverLicenceNumber: this.cachedDriver.driverLicenseNumber as string })
      .subscribe({
        next: (driver) => {
          this.userService.setCacheDriver(driver);
          this.driverDetails = this.driverDetails;
        },
        error: (error) => {
          console.error('error', error);
        }
      });
  }
}
