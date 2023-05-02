import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { CasesRoutingModule } from './cases-routing.module';
import { ClinicsComponent } from './clinics/clinics.component';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';

@NgModule({
  declarations: [ViewComponent, ListComponent, ClinicsComponent],
  imports: [CommonModule, CasesRoutingModule, SharedModule],
})
export class CasesModule {}
