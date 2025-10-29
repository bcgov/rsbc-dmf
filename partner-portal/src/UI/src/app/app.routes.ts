import { Routes } from '@angular/router';
import { DriverSearchComponent } from './driver-search/driver-search.component';
import { SearchComponent } from './search/search.component';
import { RemedialComponent } from './remedial/remedial.component';
import { AssistDriverComponent } from './assist-driver/assist-driver.component';
import { CaseSearchComponent } from './case-search/case-search.component';
import { SubmissionHistoryComponent } from './submission-history/submission-history.component';
import { LettersToDriverComponent } from './letters-to-driver/letters-to-driver.component';
import { GetHelpComponent } from './get-help/get-help.component';
import { AuthGuard } from './modules/keycloak/keycloak.guard';
import { DriverDetailsComponent } from './driver-details/driver-details.component';

export const routes: Routes = [
  { path: '', redirectTo: 'search', pathMatch: 'full' },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      { path: 'search', component: SearchComponent },
      { path: 'remedial', component: RemedialComponent },
      { path: 'driverSearch/:driverLicenceNumber', component: DriverSearchComponent },
      { path: 'assistDriver', component: AssistDriverComponent },
      { path: 'submissionHistory', component: SubmissionHistoryComponent },
      { path: 'letterToDriver', component: LettersToDriverComponent },
      { path: 'caseSearch/:caseId', component: CaseSearchComponent },
      { path: 'getHelp', component: GetHelpComponent},
      { path: 'driverDetails', component: DriverDetailsComponent}
    ],
  },
];
