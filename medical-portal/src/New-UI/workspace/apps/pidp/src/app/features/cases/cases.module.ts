import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CasesRoutingModule } from './cases-routing.module';
import { ViewComponent } from './view/view.component';
import { ListComponent } from './list/list.component';
import { SharedModule } from '../shared/shared.module';
import { ClinicsComponent } from './clinics/clinics.component';


@NgModule({
  declarations: [
    ViewComponent,
    ListComponent,
    ClinicsComponent
  ],
  imports: [
    CommonModule,
    CasesRoutingModule,
    SharedModule
  ]
})
export class CasesModule { }
