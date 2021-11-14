import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CasesRoutingModule } from './cases-routing.module';
import { ViewComponent } from './view/view.component';
import { ListComponent } from './list/list.component';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [
    ViewComponent,
    ListComponent
  ],
  imports: [
    CommonModule,
    CasesRoutingModule,
    SharedModule
  ]
})
export class CasesModule { }
