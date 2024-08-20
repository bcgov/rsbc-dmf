import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { Router } from '@angular/router';
import { DriverService } from '@app/shared/api/services';
import { SharedPortalLibComponent } from '@shared/portal-ui';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    SharedPortalLibComponent,
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

  constructor(
    private driverService: DriverService,
    private router: Router,
  ) { }

  search() {
    this.driverService
      .apiDriverInfoDriverLicenceNumberGet$Json({ driverLicenceNumber: this.driverLicenceNumber })
      .subscribe({
        next: (driver) => {
          this.router.navigateByUrl(`caseSearch?firstName=${driver.firstName}&lastName=${driver.lastName}&driverLicenceNumber=${driver.licenseNumber}`);
        },
        error:(error) => {
          console.error('error', error);
        }
      });
  }
}


