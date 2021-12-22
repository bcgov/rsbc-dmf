import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { PhsaFormViewerComponent } from './components/phsa-form-viewer/phsa-form-viewer.component';
import { SafePipe } from './pipes/safe.pipe';
import { VersionInfoComponent } from './components/version-info/version-info.component';

@NgModule({
  declarations: [    
    PhsaFormViewerComponent,
    VersionInfoComponent,
    SafePipe
  ],
  imports: [
    CommonModule,
    MaterialModule
  ],
  exports: [
    MaterialModule,
    PhsaFormViewerComponent,   
    VersionInfoComponent, 
    SafePipe
  ]
})
export class SharedModule { }
