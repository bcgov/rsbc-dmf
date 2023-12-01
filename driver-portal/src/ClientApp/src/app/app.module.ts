import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { LayoutModule } from './layout/layout.module';
import { HeaderComponent } from './layout/header/header.component';
import { SharedModule } from './shared/shared.module';
import { OAuthModule } from 'angular-oauth2-oidc';
import { ApiModule } from './shared/api/api.module';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent
  ],

  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ],
  
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),    
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
    RouterModule.forRoot([
      
    ]),
    
    
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
