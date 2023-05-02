import { LayoutModule } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { VersionInfoModule } from '@app/shared/components/version-info/version-info.module';
import { SharedModule } from '@app/shared/shared.module';

import { DmerSubmissionConfirmationDialogComponent } from './dmer-submission-confirmation-dialog/dmer-submission-confirmation-dialog.component';
import { DmerSubmissionRoutingModule } from './dmer-submission-confirmation-routing.module';
import { DmerSubmissionConfirmationComponent } from './dmer-submission-confirmation.component';

@NgModule({
  declarations: [
    DmerSubmissionConfirmationComponent,
    DmerSubmissionConfirmationDialogComponent,
  ],
  imports: [
    CommonModule,
    LayoutModule,
    MatTableModule,
    DmerSubmissionRoutingModule,
    MatIconModule,
    VersionInfoModule,
    SharedModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DmerSubmissionConfirmationModule {}
