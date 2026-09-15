import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgModel, ReactiveFormsModule } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { Router } from '@angular/router';
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
  createDriverLicenceNumber = '';
  idCode = '';
  caseSurCode = '';
  noResults: boolean = false;
  showCreateDriverForm: boolean = false;
  surcode = '';
  isSearching = false;
  isCreatingDriver = false;
  driverSearchAttempted: boolean = false;
  caseSearchAttempted: boolean = false;
  searchExecuted: boolean = false;
  createDriverMessage = '';
  private readonly createDriverSuccessMessage = 'Driver record created successfully. You may search for driver again';
  private readonly createDriverFailureMessage = "Driver record creation failed. Please check the Driver's Licence number.";

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
    this.showCreateDriverForm = false;
    this.createDriverMessage = '';
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
    this.showCreateDriverForm = false;
    this.createDriverMessage = '';
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

  openCreateDriverForm() {
    this.createDriverLicenceNumber = this.driverLicenceNumber?.trim() || '';
    this.createDriverMessage = '';
    this.showCreateDriverForm = true;
  }

  cancelCreateDriverForm() {
    this.createDriverMessage = '';
    this.showCreateDriverForm = false;
  }

  createDriverRecord() {
    const normalizedDriverLicenceNumber = this.createDriverLicenceNumber?.trim();
    if (!normalizedDriverLicenceNumber || this.isCreatingDriver) {
      return;
    }

    this.isCreatingDriver = true;
    this.createDriverMessage = '';

    this.caseManagementService
      .createDriverRecord(normalizedDriverLicenceNumber)
      .pipe(finalize(() => (this.isCreatingDriver = false)))
      .subscribe({
        next: (response: { success?: boolean; Success?: boolean; message?: string; Message?: string }) => {
          const isSuccess = response.success ?? response.Success ?? false;
          const responseMessage = response.message ?? response.Message ?? this.createDriverFailureMessage;

          if (isSuccess) {
            this.createDriverMessage = this.createDriverSuccessMessage;
            this.driverLicenceNumber = normalizedDriverLicenceNumber;
            this.showCreateDriverForm = false;
            this.noResults = false;
            return;
          }

          this.createDriverMessage = responseMessage;
        },
        error: (error: { error?: { success?: boolean; Success?: boolean; message?: string; Message?: string } }) => {
          const errorBody = error?.error;
          const isSuccess = errorBody?.success ?? errorBody?.Success ?? false;
          if (isSuccess) {
            this.createDriverMessage = this.createDriverSuccessMessage;
            this.driverLicenceNumber = normalizedDriverLicenceNumber;
            this.showCreateDriverForm = false;
            this.noResults = false;
            return;
          }

          this.createDriverMessage = errorBody?.message ?? errorBody?.Message ?? this.createDriverFailureMessage;
          console.error('Create driver error:', error);
        }
      });
  }
}




