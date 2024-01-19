import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CaseComponent } from './case/case.component';
import { CaseDetailsComponent } from './case-details/case-details.component';
import { SubmissionHistoryComponent } from './submission-history/submission-history.component';
import { LettersToDriverComponent } from './letters-to-driver/letters-to-driver.component';
import { LetterDetailsComponent } from './letter-details/letter-details.component';

const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'account', component: AccountComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'cases', component: CaseComponent },
  { path: 'caseDetails', component: CaseDetailsComponent },
  { path: 'submissionHistory', component: SubmissionHistoryComponent },
  { path: 'lettersToDriver', component: LetterDetailsComponent },
];

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
