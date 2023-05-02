import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';

import { DashboardModule } from '@bcgov/shared/ui';

//import { Dashboard2Component } from '../dashboard/dashboard.component';
import { PortalDashboardComponent } from './components/portal-dashboard/portal-dashboard.component';
import { ShellRoutingModule } from './shell-routing.module';

@NgModule({
  declarations: [PortalDashboardComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [ShellRoutingModule, DashboardModule, CommonModule],
})
export class ShellModule {}
