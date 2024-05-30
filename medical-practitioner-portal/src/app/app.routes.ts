import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CaseDetailsComponent } from './case-details/case-details.component';
import { AccountComponent } from './account/account.component';
import { GetHelpComponent } from './get-help/get-help.component';
import { CaseSubmissionsComponent } from './case-submissions/case-submissions.component';
import { AuthenticationGuard } from './features/auth/guards/authentication.guard';
import { DeniedComponent } from './denied/denied.component';
import { ProfileComponent } from './profile/profile.component';

export const ROUTE_DENIED = 'denied';
export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: ROUTE_DENIED, component: DeniedComponent },
  {
    path: '',
    canActivateChild: [AuthenticationGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'caseDetails/:caseId', component: CaseDetailsComponent },
      { path: 'account', component: AccountComponent },
      { path: 'getHelp', component: GetHelpComponent },
      { path: 'caseSubmissions', component: CaseSubmissionsComponent },
      { path: 'profile', component: ProfileComponent },
    ],
  },
];
