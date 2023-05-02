import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

import { DashboardModule } from '@bcgov/shared/ui';

import { SupportErrorRoutingModule } from './support-error-routing.module';
import { SupportErrorPage } from './support-error.page';

@NgModule({
  declarations: [SupportErrorPage],
  imports: [SupportErrorRoutingModule, DashboardModule, MatButtonModule],
})
export class SupportErrorModule {}
