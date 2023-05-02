import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedUiModule } from '@bcgov/shared/ui';

import { PhsaFormViewerComponent } from './components/phsa-form-viewer/phsa-form-viewer.component';
import { MaterialModule } from './material.module';
import { SafePipe } from './pipes/safe.pipe';

@NgModule({
  declarations: [PhsaFormViewerComponent, SafePipe],
  imports: [CommonModule, SharedUiModule],
  exports: [
    CommonModule,
    SharedUiModule,
    MaterialModule,
    PhsaFormViewerComponent,
    SafePipe,
  ],
})
export class SharedModule {}
