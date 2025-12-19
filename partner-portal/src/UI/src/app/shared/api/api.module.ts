/* tslint:disable */
/* eslint-disable */
import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiConfiguration, ApiConfigurationParams } from './api-configuration';

import { ApplicationVersionInfoService } from './services/application-version-info.service';
import { CallbackService } from './services/callback.service';
import { CasesService } from './services/cases.service';
import { CommentsService } from './services/comments.service';
import { ConfigService } from './services/config.service';
import { DocumentService } from './services/document.service';
import { DocumentTypeService } from './services/document-type.service';
import { DriverService } from './services/driver.service';
import { RemedialService } from './services/remedial.service';
import { UserService } from './services/user.service';

/**
 * Module that provides all services and configuration.
 */
@NgModule({
  imports: [],
  exports: [],
  declarations: [],
  providers: [
    ApplicationVersionInfoService,
    CallbackService,
    CasesService,
    CommentsService,
    ConfigService,
    DocumentService,
    DocumentTypeService,
    DriverService,
    RemedialService,
    UserService,
    ApiConfiguration
  ],
})
export class ApiModule {
  static forRoot(params: ApiConfigurationParams): ModuleWithProviders<ApiModule> {
    return {
      ngModule: ApiModule,
      providers: [
        {
          provide: ApiConfiguration,
          useValue: params
        }
      ]
    }
  }

  constructor( 
    @Optional() @SkipSelf() parentModule: ApiModule,
    @Optional() http: HttpClient
  ) {
    if (parentModule) {
      throw new Error('ApiModule is already loaded. Import in your base AppModule only.');
    }
    if (!http) {
      throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
      'See also https://github.com/angular/angular/issues/20575');
    }
  }
}
