import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { LoginService } from '../shared/services/login.service';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  isEditView = false;

  accountForm = this.fb.group({
    mail: [false],
    firstName: [''],
    lastName: [''],
    emailAddress: [''],
    driverLicenceNumber: [''],
  });

  isCreateProfile = this.route.snapshot.routeConfig?.path === 'create-profile';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private caseManagementService: CaseManagementService,
    private loginService: LoginService,
    private _http: HttpClient,
    public _snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    if (this.loginService.userProfile?.id) {
      this.getuserDetails(this.loginService.userProfile?.id as string);
    }
    this.accountForm.disable();

    if (this.isCreateProfile) {
      this.accountForm.controls.emailAddress.enable();
      this.accountForm.controls.driverLicenceNumber.enable();
    }
  }

  getuserDetails(driverId: string) {
    this.loginService.getUserProfile().subscribe((user) => {
      this.accountForm.patchValue(user);
    });
  }

  onEdit() {
    this.accountForm.controls.emailAddress.enable();
    this.isEditView = true;
  }

  onUpdate() {
    this.caseManagementService
      .userRegistration({
        body: {
          //driverLicenseNumber: this.accountForm.value.driverLicenceNumber,
          email: this.accountForm.value.emailAddress,
        },
      })
      .subscribe((res) => {
        console.log(res);
      });
  }

  onRegister() {
    console.log('Registering...');
    this.caseManagementService
      .userRegistration({
        body: {
          driverLicenseNumber: this.accountForm.value.driverLicenceNumber,
          email: this.accountForm.value.emailAddress,
        },
      })
      .subscribe({
        error: (err) => {
          console.log(typeof err.status);
          if (err.status === 401) {
            this._snackBar.open(
              'Unable To Register. Please Check that you have entered your Driver License Number correctly. The name and birthdate on your Driver Licence must match the details on your BC Services Card',
              'Close',
              {
                horizontalPosition: 'center',
                verticalPosition: 'top',
                panelClass: ['warning']
              }
            );
          }
        },
      });
  }
}
