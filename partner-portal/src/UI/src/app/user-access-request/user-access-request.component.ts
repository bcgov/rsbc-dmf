import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-access-request',
  standalone: true,
  imports: [
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './user-access-request.component.html',
  styleUrl: './user-access-request.component.scss'
})
export class UserAccessRequestComponent {
  userAccessForm: FormGroup;

  constructor(private formBuilder: FormBuilder) {
    this.userAccessForm = this.formBuilder.group({
      givenName: ['', [Validators.required]],
      secondGivenName: [''], // Optional field
      thirdGivenName: [''], // Optional field
      surname: ['', [Validators.required]],
      address: ['', [Validators.required]],
      city: ['', [Validators.required]],
      province: ['', [Validators.required]],
      phoneNumber: ['']
    });
  }

  onSubmit() {
    if (this.userAccessForm.valid) {
      console.log('Form Submitted:', this.userAccessForm.value);
      // Add your submit logic here
    } else {
      console.log('Form is invalid');
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched() {
    Object.keys(this.userAccessForm.controls).forEach(key => {
      const control = this.userAccessForm.get(key);
      control?.markAsTouched();
    });
  }
}
