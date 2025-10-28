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

  displayedColumns: string[] = ['issuedDate', 'issuedBy', 'physiciansGuide', 'examDate', 'recvdDate', 'disposition'];
  mxmDataSource: any[] = [
    { issuedDate: 'Test Date', issuedBy: 'Test User', physiciansGuide: 'Test Guide', examDate: 'Test Exam', recvdDate: 'Test Recvd', disposition: 'Test Disposition' }
    // Add your actual data here
  ];
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
        },
        error: (error) => {
          console.error('error', error);
        }
      });
  }
}
