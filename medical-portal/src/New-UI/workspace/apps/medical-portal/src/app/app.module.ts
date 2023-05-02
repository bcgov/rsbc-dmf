import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/compiler';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PidpDataModelModule } from '@pidp/data-model';
import { PidpPresentationModule } from '@pidp/presentation';
import { DynamicDialogModule } from 'primeng/dynamicdialog';

import { AppRoutingModule } from '@app/app-routing.module';
import { AppComponent } from '@app/app.component';

import { CoreModule } from '@core/core.module';

import { SharedUiModule } from '../../../../libs/shared/ui/src/lib/shared-ui.module';
import { DmerSubmissionConfirmationModule } from './features/dmer-submission-confirmation/dmer-submission-confirmation.module';
import { EndorsementLicenseModule } from './features/portal/endorsement/License/endorsement-license/endorsement-license.module';
//import { EndorsementComponentComponent } from './features/portal/endorsement/endorsement-component.component';
import { EndorsementModuleModule } from './features/portal/endorsement/endorsement-module.module';

@NgModule({
  declarations: [AppComponent],
  bootstrap: [AppComponent],
  imports: [
    AppRoutingModule,
    CoreModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    EndorsementModuleModule,

    // Import DMFT modules in AppModule so that singleton services are injected at the top of the module hierarchy.
    PidpDataModelModule,
    PidpPresentationModule,
    SharedUiModule,
  ],
})
export class AppModule {}
