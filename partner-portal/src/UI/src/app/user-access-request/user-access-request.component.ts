import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { ProfileService } from '@app/shared/api/services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-access-request',
  standalone: true,
  imports: [
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './user-access-request.component.html',
  styleUrl: './user-access-request.component.scss'
})
export class UserAccessRequestComponent {
  userAccessForm: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private profileService: ProfileService,
    private router: Router
    ,
    private snackBar: MatSnackBar
  ) {
    this.userAccessForm = this.formBuilder.group({
      givenName: ['', [Validators.required]],
      secondGivenName: [''], // Optional field
      thirdGivenName: [''], // Optional field
      surname: ['', [Validators.required]],
      addressFirstLine: ['', [Validators.required]],
      addressSecondLine: [''],
      addressThirdLine: [''],
      city: ['', [Validators.required]],
      province: ['', [Validators.required]],
      country : ['', [Validators.required]],
      postalCode: ['', [Validators.required]],
      phoneNumber: [''],
      cellPhoneNumber: [''],
      emailAddress:['']
    });
  }

  onSubmit() {
    if (this.userAccessForm.valid) {
      console.log('Form Submitted:', this.userAccessForm.value);
      // Add your submit logic here
      this.profileService.apiProfileRegisterPut$Json({ body: this.userAccessForm.value }).subscribe({
        next: (response) => {
          console.log('User access request submitted successfully:', response);
          // show a brief confirmation to the user
          this.snackBar.open('User access request submitted successfully', 'Close', { duration: 5000 });

          // Navigate to the search Page
          this.router.navigate(['/search']);

        },
        error: (error) => {
          console.error('Error submitting user access request:', error);
          // optionally surface the error to the user
          this.snackBar.open('Error submitting user access request', 'Close', { duration: 5000 });
        }
      });
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
