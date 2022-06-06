import { NgModule } from '@angular/core';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LayoutModule } from '@angular/cdk/layout';
import {MatDialogModule} from '@angular/material/dialog';
import { SharedModule } from './shared/shared.module';
import { LayoutModule as PortalLayoutModule } from './layout/layout.module';
import { ApiModule } from './shared/api/api.module';
import { HttpClientModule } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { OAuthModule } from 'angular-oauth2-oidc';
import {
  FontAwesomeModule,
  FaIconLibrary,
} from '@fortawesome/angular-fontawesome';
import { faSearch } from '@fortawesome/free-solid-svg-icons';
import { NgBusyModule } from 'ng-busy';
import { CaseAssistanceComponent } from './case-assistance/case-assistance.component';
import { CaseDetailsComponent } from './case-details/case-details.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { EditUserProfileDialogComponent } from './user-profile/edit-user-profile-dialog/edit-user-profile-dialog.component';
import { CreateMedicalPractitionerAssociationDialogComponent } from './user-profile/create-medical-practitioner-association-dialog/create-medical-practitioner-association-dialog.component';
import { ManageMedicalPractitionerAssociationDialogComponent } from './user-profile/manage-medical-practitioner-association-dialog/manage-medical-practitioner-association-dialog.component';
import { CreateMedicalStaffAssociationDialogComponent } from './user-profile/create-medical-staff-association-dialog/create-medical-staff-association-dialog.component';
import { EditMedicalStaffAssociationDialogComponent } from './user-profile/edit-medical-staff-association-dialog/edit-medical-staff-association-dialog.component';
import { ManageMedicalStaffAssociationDialogComponent } from './user-profile/manage-medical-staff-association-dialog/manage-medical-staff-association-dialog.component';
import { RemoveMedicalStaffAssociationDialogComponent } from './user-profile/remove-medical-staff-association-dialog/remove-medical-staff-association-dialog.component';
import { EditMedicalPractitionerUserProfileDialogComponent } from './user-profile/edit-medical-practitioner-user-profile-dialog/edit-medical-practitioner-user-profile-dialog.component';
import { CreateMedicalPractitionerRoleAssociationDialogComponent } from './user-profile/create-medical-practitioner-role-association-dialog/create-medical-practitioner-role-association-dialog.component';
import { EditMedicalPractitionerRoleAssociationDialogComponent } from './user-profile/edit-medical-practitioner-role-association-dialog/edit-medical-practitioner-role-association-dialog.component';
import { ManageMedicalPractitionerRoleAssociationDialogComponent } from './user-profile/manage-medical-practitioner-role-association-dialog/manage-medical-practitioner-role-association-dialog.component';





@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    CaseAssistanceComponent,
    CaseDetailsComponent,
    UserProfileComponent,
    EditUserProfileDialogComponent,
    CreateMedicalPractitionerAssociationDialogComponent,
    ManageMedicalPractitionerAssociationDialogComponent,
    CreateMedicalStaffAssociationDialogComponent,
    EditMedicalStaffAssociationDialogComponent,
    ManageMedicalStaffAssociationDialogComponent,
    RemoveMedicalStaffAssociationDialogComponent,
    EditMedicalPractitionerUserProfileDialogComponent,
    CreateMedicalPractitionerRoleAssociationDialogComponent,
    EditMedicalPractitionerRoleAssociationDialogComponent,
    ManageMedicalPractitionerRoleAssociationDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    MatDialogModule,
    SharedModule,
    PortalLayoutModule,
    HttpClientModule,
    NgBusyModule.forRoot({
      backdrop: true,
      wrapperClass: 'ng-busy',

    }),
    FlexLayoutModule,
    ApiModule.forRoot({ rootUrl: '.' }),
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
        customUrlValidation: url => url.toLowerCase().includes('/api/') && !url.toLowerCase().endsWith('/config'),
      }
    })
  ],
  providers: [
    { provide: APP_BASE_HREF, useFactory: (s: PlatformLocation) => {
      var result = s.getBaseHrefFromDOM()
      let hasTrailingSlash = result[result.length-1] === '/';
      if(hasTrailingSlash) {
        result = result.substr(0, result.length - 1);
      }
      return result;
    },
      deps: [PlatformLocation] }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(library: FaIconLibrary) {
    library.addIcons(
      faSearch
    );
  }


}
