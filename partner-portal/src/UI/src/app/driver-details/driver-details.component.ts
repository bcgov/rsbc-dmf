import { DatePipe } from '@angular/common';
import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Driver, UserContext } from '@app/shared/api/models';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';

@Component({
  selector: 'app-driver-details',
  standalone: true,
  imports: [MatCardModule, MatTableModule, DatePipe],
  templateUrl: './driver-details.component.html',
  styleUrl: './driver-details.component.scss'
})
export class DriverDetailsComponent {
  cachedDriver = this.userService.getCachedriver();
  driverDetails: Driver = {};
  driverLicenceNumber = this.route.snapshot.params['driverLicenceNumber'];

  mxmColumns: string[] = ['medicalIssueDate', 'issuingOffice', 'physicianGuideNumber', 'medicalExamDate' ,'receivedDate', 'medicalDisposition'];
  mxmDataSource: any[] = [];

  xsStatusColumns: string[] = ['expandedStatus', 'newExpandedStatus'];
  xsStatusDataSource: any[] = [];
  constructor(
    private userService: UserService,
    private caseManagementService: CaseManagementService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.getDriverDetails();
  }

  getDriverDetails() {
    this.caseManagementService
      .searchByDriver({ driverLicenceNumber: this.driverLicenceNumber })
      .subscribe({
        next: (driver) => {
          this.userService.setCacheDriver(driver);
          this.driverDetails = driver;
          // Update the XS Status data source with actual driver status data
          this.xsStatusDataSource = driver.status || [];
          // Update the MXM Information data source with actual medical data
          this.mxmDataSource = driver.medicals || [];
        },
        error: (error) => {
          console.error('error', error);
        }
      });
  }
}
