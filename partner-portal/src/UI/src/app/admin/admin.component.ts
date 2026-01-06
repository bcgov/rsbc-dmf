import { CommonModule } from '@angular/common';
import { Component, importProvidersFrom } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatTableModule } from '@angular/material/table';
import { AdminSearch } from '../app.model';
import { CaseManagementService } from '@app/shared/services/case-management/case-management.service';
import { User, UserType } from '@app/shared/api/models';
import { AdminDetailsComponent } from './admin-details/admin-details.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from '@app/app.component';
bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(BrowserAnimationsModule)
  ]
});
@Component({
  selector: 'app-admin',
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
      MatCheckboxModule,
      MatRadioModule,
      MatTableModule,
      AdminDetailsComponent,
      NgxSpinnerModule,
    ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {
  adminSearch = new AdminSearch();
  noResults: boolean = false;
  users: User[] = [];
  userColumns: string[] = ['active', 'lastName', 'firstName', 'identityId', 'domain',  'roles','details'];
  showAdminDetails: boolean = false;
  userDetails: User = {} as User;
  authorizeUser: boolean = false;
  constructor(
    private caseManagementService: CaseManagementService,
    private spinner: NgxSpinnerService){
  }

  ngOnInit(){
    this.adminSearch.activeUser = -1;
  }

  getUsers(){
    this.spinner.show('main');
    this.noResults = false;
    var userSearch =  {
      externalSystemUserId: this.adminSearch.userId,
      firstName: this.adminSearch.givenName,
      lastName: this.adminSearch.surname,
      unauthorizedOnly:this.adminSearch.unauthorizedOnly,
      userId: this.adminSearch.userId,
      activeUser: this.adminSearch.activeUser,
      userType: UserType.$2 // Partner Portal Users
    }

    this.caseManagementService
      .getUsers( { body: userSearch } )
      .subscribe({
        next: (users) => {
          this.users = users;
          this.spinner.hide('main');
        },
        error: (error) => {
          this.noResults = true;
          this.spinner.hide('main');
          console.error('error', error);
        }
      });
  }

  exportUsers(){
    this.spinner.show('main');
    this.caseManagementService.exportUsers( { body: this.users } )
      .subscribe({
        next: (response: Blob | MediaSource) => { 
          console.log('Export initiated', response);
          const url = window.URL.createObjectURL(response);

      const a = document.createElement('a');
      a.href = url;
      a.download = 'partner-portal-users-' + new Date().toISOString().split('T')[0] + '.csv'; // filename fallback
      a.click();

      window.URL.revokeObjectURL(url);
          this.spinner.hide('main');
        },
        error: () => {
          this.spinner.hide('main');
        }
      });
  }

  viewUserDetails(element: User){
    this.userDetails = element;
    this.showAdminDetails = true;
    if(element.authorized == false){
      this.authorizeUser = true;
    }
  }

  getRolesAsString(roles: any[]): string {
    return roles.map(r => r.roleID).join(", ");
  }

  hideAdminDetails(){
    this.showAdminDetails = false;
    this.clearData();
  }

  formatDate(dateString: string | null | undefined): string {
    return dateString?.split('T')[0] ?? '';
  }

  clearData(){
    this.users = [] as User[];
    this.adminSearch = new AdminSearch();
  }
}
