import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgModel, ReactiveFormsModule } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { Router } from '@angular/router';
import { CaseSearch } from '@app/shared/api/models';
import { DriverService } from '@app/shared/api/services';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { UserService } from '@app/shared/services/user.service';
import { finalize } from 'rxjs';

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
    MatButton, 
    MatError
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
  driverLicenceNumber = '';
  idCode = '';
  caseSurCode = '';
  noResults: boolean = false;
  surcode = '';
  isSearching = false;
  driverSearchAttempted: boolean = false;
  caseSearchAttempted: boolean = false;
  searchExecuted: boolean = false;

  constructor(
    private caseManagementService: CaseManagementService,
    private router: Router,
    private userService: UserService
  ) { }

  search(driverLicenseControl: NgModel, surCodeControl: NgModel) {
    this.driverSearchAttempted = true;
    driverLicenseControl.control.markAsTouched();
    surCodeControl.control.markAsTouched();

    if (this.isSearching) {
      return;
    }

    const normalizedDriverLicenceNumber = this.driverLicenceNumber?.trim();
    const normalizedSurcode = this.surcode?.trim().toUpperCase();
    
    // Check if form is valid before making API call
    if (!normalizedDriverLicenceNumber || !normalizedSurcode) {
      return;
    }

    this.driverLicenceNumber = normalizedDriverLicenceNumber;
    this.surcode = normalizedSurcode;
    
    this.searchExecuted = true;
    this.noResults = false;
    this.isSearching = true;

    this.caseManagementService
      .searchByDriver({ 
        driverLicenceNumber: normalizedDriverLicenceNumber,
        surCode: normalizedSurcode
      })
      .pipe(finalize(() => (this.isSearching = false)))
      .subscribe({
        next: (driver) => {
          this.userService.setCacheDriver(driver);
          this.router.navigate(['/driverSearch', normalizedDriverLicenceNumber as string], {
            queryParams: { surcode: normalizedSurcode }
          });
        },
        error: (error) => {
          this.noResults = true;
          console.error('Search error:', error);
        }
      });
  }


  searchByCaseId(caseIdControl: NgModel, caseSurCodeControl: NgModel){
    this.caseSearchAttempted = true;
    caseIdControl.control.markAsTouched();
    caseSurCodeControl.control.markAsTouched();
    if (!this.idCode?.trim() || !this.caseSurCode?.trim()) {
      return;
    }

    const effectiveCaseSurCode = this.caseSurCode.trim();
    this.noResults = false;
    this.caseManagementService.searchByCaseId({
      idCode: this.idCode,
      surCode: effectiveCaseSurCode
    })
    .subscribe({
      next: (caseDetails) => {
        this.router.navigate(['/caseSearch', this.idCode as string], {
          state: caseDetails,
          queryParams: { surcode: effectiveCaseSurCode }
        });
      },
      error: (error) => {
        this.noResults = true;
        console.error('error', error);
      }
  
    });
  }
}




