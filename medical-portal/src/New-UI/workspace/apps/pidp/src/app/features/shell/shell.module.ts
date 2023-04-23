import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { DashboardModule } from '@bcgov/shared/ui';

//import { DashboardComponent } from './components/portal-dashboard/dashboard.component';
import { ShellRoutingModule } from './shell-routing.module';

@NgModule({
  imports: [ShellRoutingModule, DashboardModule, CommonModule],
})
export class ShellModule {}
