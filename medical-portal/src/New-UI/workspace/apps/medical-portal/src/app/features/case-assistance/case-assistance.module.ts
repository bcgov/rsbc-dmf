import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { MatTableModule } from '@angular/material/table';

import { LayoutModule } from '@app/layout/layout.module';
import { VersionInfoModule } from '@app/shared/components/version-info/version-info.module';
import { SharedModule } from '@app/shared/shared.module';

import { CaseAssistanceComponent } from './case-assistance.component';
import { CaseAssistanceRoutingModule } from './case-assitance-routing.module';

@NgModule({
  declarations: [CaseAssistanceComponent],
  imports: [
    CommonModule,
    CaseAssistanceRoutingModule,
    LayoutModule,
    MatTableModule,
    VersionInfoModule,
    SharedModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CaseAssistanceModule {}
