import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CaseComponent } from './case/case.component';

const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'account', component: AccountComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'cases', component: CaseComponent },
];

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
