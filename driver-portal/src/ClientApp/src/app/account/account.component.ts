import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { LoginService } from '../shared/services/login.service';

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
    private loginService: LoginService
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

  onSubmit() {
    //this.loginService.
    console.log(this.accountForm.value);
  }
}
