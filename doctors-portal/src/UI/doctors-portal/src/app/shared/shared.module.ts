import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { PhsaFormViewerComponent } from './components/phsa-form-viewer/phsa-form-viewer.component';
import { SafePipe } from './pipes/safe.pipe';

@NgModule({
  declarations: [
    PhsaFormViewerComponent,
    SafePipe
  ],
  imports: [
    CommonModule,
    MaterialModule
  ],
  exports: [
    MaterialModule,
    PhsaFormViewerComponent,
    SafePipe
  ]
})
export class SharedModule { }
