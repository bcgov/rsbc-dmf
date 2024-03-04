import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CaseComponent } from './case/case.component';
import { CaseDetailsComponent } from './case-details/case-details.component';
import { SubmissionHistoryComponent } from './submission-history/submission-history.component';
import { LetterDetailsComponent } from './letter-details/letter-details.component';
import { UserRegistrationComponent } from './user-registration/user-registration.component';
import { GetAssistanceComponent } from './get-assistance/get-assistance.component';

const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'account',
    component: AccountComponent,
  },
  {
    path: 'create-profile',
    component: AccountComponent,
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
  },
  { path: 'cases', component: CaseComponent },
  { path: 'caseDetails', component: CaseDetailsComponent },
  { path: 'submissionHistory', component: SubmissionHistoryComponent },
  { path: 'lettersToDriver', component: LetterDetailsComponent },
  { path: 'userRegistration', component: UserRegistrationComponent },
  { path: 'getAssistance', component: GetAssistanceComponent },
];

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
