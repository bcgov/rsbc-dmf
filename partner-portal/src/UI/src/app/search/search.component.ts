import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { DriverService } from '@app/shared/api/services';

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

  constructor(private driverService: DriverService) { }

  search() {
    console.log("driver licence number", this.driverLicenceNumber);
    this.driverService
      .apiDriverInfoDriverLicenceNumberGet$Json({ driverLicenceNumber: this.driverLicenceNumber })
      .subscribe((driver) => {
        console.log('driver', driver);
      });
  }
}


