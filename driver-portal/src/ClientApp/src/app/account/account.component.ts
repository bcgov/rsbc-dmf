/* eslint-disable @typescript-eslint/no-unused-vars */
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  Validators,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { LoginService } from '../shared/services/login.service';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatInput } from '@angular/material/input';
import { MatFormField, MatError } from '@angular/material/form-field';
import { MatButton } from '@angular/material/button';
import { NgIf, NgClass } from '@angular/common';
import { error } from 'console';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
  standalone: true,
  imports: [
    NgIf,
    MatButton,
    ReactiveFormsModule,
    FormsModule,
    MatFormField,
    MatInput,
    NgClass,
    MatError,
  ],
})
export class AccountComponent implements OnInit {
  isEditView = false;
  onEditProfile = false;
  acceptControl = new FormControl(false);

  accountForm = this.fb.group({
    notifyByEmail: [false],
    notifyByMail: [false],
    firstName: [''],
    lastName: [''],
    middleName: [''],
    emailAddress: ['', Validators.required],
    driverLicenseNumber: ['', Validators.maxLength(8)],
    addressLine1: [''],
    city: [''],
    province: [''],
    postal: [''],
    country: [''],
  });

  isCreateProfile = this.route.snapshot.routeConfig?.path === 'create-profile';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private caseManagementService: CaseManagementService,
    private loginService: LoginService,
    private _http: HttpClient,
    public _snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit() {
    if (this.loginService.userProfile?.id) {
      this.getuserDetails(this.loginService.userProfile?.id as string);
    }
    this.getDriverAddress();
    this.accountForm.disable();

    if (this.isCreateProfile) {
      this.onEditProfile = true;
      this.accountForm.controls.emailAddress.enable();
      this.accountForm.controls.driverLicenseNumber.enable();
      this.accountForm.controls.notifyByEmail.enable();
      this.accountForm.controls.notifyByMail.enable();
    }
  }

  getuserDetails(driverId: string) {
    this.loginService.getUserProfile().subscribe((user) => {
      this.accountForm.patchValue(user);
    });
  }

  getDriverAddress() {
    this.caseManagementService.getDriverAddress({}).subscribe({
      next: (userAddress) => {
        this.accountForm.patchValue(userAddress as any);
      },
      error: (err) => {
        if (err.status === 401) {
          this._snackBar.open(
            'Unable To Register. Please check that the address matches ICBC address',
            'Close',
            {
              horizontalPosition: 'center',
              verticalPosition: 'top',
              duration: 5000,
            }
          );
        }
      },
    });
  }
  onEdit() {
    this.accountForm.controls.emailAddress.enable();
    this.isEditView = true;
    this.onEditProfile = true;
  }

  isUpdatingProfile = false;
  onUpdate() {
    if (this.accountForm.invalid) {
      this.accountForm.markAllAsTouched();
      return;
    }
    if (this.isUpdatingProfile) {
      return;
    }

    this.isUpdatingProfile = true;
    this.caseManagementService
      .updateDriverProfile({
        body: {
          email: this.accountForm.value.emailAddress,
        },
      })
      .subscribe((res) => {
        this._snackBar.open('Successfully Updated Email Address', 'Close', {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 5000,
        });
        this.isUpdatingProfile = false;
      });
  }

  onCancel() {
    this.accountForm.controls.emailAddress.disable();
    this.isEditView = false;
    this.onEditProfile = false;
  }

  onRegister() {
    if (this.accountForm.invalid) {
      this.accountForm.markAllAsTouched();
      return;
    }

    this.caseManagementService
      .userRegistration({
        body: {
          driverLicenseNumber: this.accountForm.value.driverLicenseNumber,
          email: this.accountForm.value.emailAddress,
          notifyByEmail: this.accountForm.value.notifyByEmail as boolean,
          notifyByMail: this.accountForm.value.notifyByMail as boolean,
        },
      })
      .subscribe({
        next: () => {
          this._snackBar.open('Registration successful', 'Close', {
            horizontalPosition: 'center',
            verticalPosition: 'top',
            duration: 5000,
          });
          this.router.navigate(['/dashboard']);
          location.reload();
        },
        error: (err) => {
          if (err.status === 401) {
            this._snackBar.open(
              'Unable To Register. Please Check that you have entered your Driver License Number correctly. The name and birthdate on your Driver Licence must match the details on your BC Services Card',
              'Close',
              {
                horizontalPosition: 'center',
                verticalPosition: 'top',
                duration: 5000,
              }
            );
          }
        },
      });
  }
}
