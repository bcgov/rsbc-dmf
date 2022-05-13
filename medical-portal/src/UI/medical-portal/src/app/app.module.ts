import { NgModule } from '@angular/core';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LayoutModule } from '@angular/cdk/layout';
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
import { CaseAssistanceComponent } from './case-assistance/case-assistance.component';




@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    CaseAssistanceComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    SharedModule,
    PortalLayoutModule,
    HttpClientModule,
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
