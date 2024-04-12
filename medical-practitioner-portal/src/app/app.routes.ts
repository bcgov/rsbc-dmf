import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CaseDetailsComponent } from './case-details/case-details.component';
import { AccountComponent } from './account/account.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    component: DashboardComponent,
  },
  { path: 'caseDetails', component: CaseDetailsComponent },
  {path:'account', component: AccountComponent}
];
