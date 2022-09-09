import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CaseAssistanceComponent } from './case-assistance/case-assistance.component';
import { CaseDetailsComponent } from './case-details/case-details.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DmerSubmissionConfirmationComponent } from './dmer-submission-confirmation/dmer-submission-confirmation.component';
import { UserProfileComponent } from './user-profile/user-profile.component';

const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'cases', loadChildren: () => import('./cases/cases.module').then(m => m.CasesModule) },
  { path: 'users', loadChildren: () => import('./users/users.module').then(m => m.UsersModule) },
  { path: 'notifications', loadChildren: () => import('./notifications/notifications.module').then(m => m.NotificationsModule) },
  { path:'caseAssistance', component:CaseAssistanceComponent },
  {path:'dmerSubmissionConfirmation', component:DmerSubmissionConfirmationComponent},
  {path:'caseDetails/:id', component:CaseDetailsComponent},
  { path: 'userProfile', component: UserProfileComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
