import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { Router } from '@angular/router';
import { CaseSearch } from '@app/shared/api/models';
import { DriverService } from '@app/shared/api/services';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserServices } from '@app/shared/services/user.service';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    MatCardModule,
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormField,
    MatInput,
    MatButton
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
  driverLicenceNumber = '';
  idCode = '';
  noResults: boolean = false;
  surcode = '';

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router,
    private userService: UserServices
  ) { }

  search() {
    this.noResults = false;
    
    // Ensure surcode is not empty - use a default value if empty
    const effectiveSurcode = this.surcode?.trim();

    this.caseManagementService
      .searchByDriver({ 
        driverLicenceNumber: this.driverLicenceNumber,
        surCode: effectiveSurcode
      })
      .subscribe({
        next: (driver) => {
          this.userService.setCacheDriver(driver);
          this.router.navigate(['/driverSearch', this.driverLicenceNumber as string], {
            queryParams: { surcode: this.surcode }
          });
        },
        error: (error) => {
          this.noResults = true;
          console.error('Search error:', error);
        }
      });
  }


  searchByCaseId(){
    this.noResults = false;
    this.caseManagementService.searchByCaseId({idCode: this.idCode})
    .subscribe({
      next: (caseDetails) => {
        this.router.navigate(['/caseSearch', this.idCode as string], {state: caseDetails});
      },
      error: (error) => {
        this.noResults = true;
        console.error('error', error);
      }
  
    });
  }
}




