import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { Router } from '@angular/router';
import { DriverService } from '@app/shared/api/services';
import { UserService } from '@app/shared/services/user.service';

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

  constructor(
    private driverService: DriverService,
    private router: Router,
    private userService: UserService
  ) { }

  search() {
    this.driverService
      .apiDriverInfoDriverLicenceNumberGet$Json({ driverLicenceNumber: this.driverLicenceNumber })
      .subscribe({
        next: (driver) => {
          this.userService.setCacheDriver(driver);
          this.router.navigateByUrl('/driverSearch');
        },
        error: (error) => {
          console.error('error', error);
        }
      });
  }
}


