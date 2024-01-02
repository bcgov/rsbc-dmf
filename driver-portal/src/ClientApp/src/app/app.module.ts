import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { OAuthModule } from 'angular-oauth2-oidc';
import { ApiModule } from './shared/api/api.module';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { AccountComponent } from './account/account.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {CoreUiModule, LayoutModule} from '@shared/core-ui';
import { DashboardComponent } from './dashboard/dashboard.component'
import { MaterialModule } from './shared/material.module';
import { RecentCaseComponent } from './recent-case/recent-case.component';
import { CaseComponent } from './case/case.component';
import { CaseTypeComponent } from './case-definations/case-type/case-type.component';
import { CaseStatusComponent } from './case-definations/case-status/case-status.component';
import { DmerTypeComponent } from './case-definations/dmer-type/dmer-type.component';

@NgModule({
  declarations: [
    AppComponent,
    AccountComponent,
    DashboardComponent,
    RecentCaseComponent,
    CaseComponent,
    CaseTypeComponent,
    CaseStatusComponent,
    DmerTypeComponent
  ],

  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
  
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),   
    BrowserAnimationsModule,
    AppRoutingModule, 
    HttpClientModule,
    FormsModule,
    LayoutModule,
    SharedModule,
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: false,
        //customUrlValidation: url => url.toLowerCase().includes('/api/') && !url.toLowerCase().endsWith('/config'),
      }
    }),
    CoreUiModule,
    RouterModule.forRoot([
      
    ]),
    AppRoutingModule,
    MaterialModule
    
    
  ],
  providers: [
    { provide: APP_BASE_HREF, useFactory: (s: PlatformLocation) => {
      let result = s.getBaseHrefFromDOM()
      const hasTrailingSlash = result[result.length-1] === '/';
      if(hasTrailingSlash) {
        result = result.substr(0, result.length - 1);
      }
      return result;
    },
      deps: [PlatformLocation] }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
