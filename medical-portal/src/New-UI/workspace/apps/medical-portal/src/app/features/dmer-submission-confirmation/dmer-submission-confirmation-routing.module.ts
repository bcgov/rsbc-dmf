import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SetDashboardTitleGuard } from '@pidp/presentation';

import { DmerSubmissionConfirmationComponent } from './dmer-submission-confirmation.component';

const routes: Routes = [
  {
    path: '',
    component: DmerSubmissionConfirmationComponent,
    canActivate: [SetDashboardTitleGuard],
    data: {
      title: 'Dmer Submisison',
      setDashboardTitleGuard: {
        titleText: 'Dmer Submisison',
        titleDescriptionText: 'Dmer Submisison Confirmation',
      },
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DmerSubmissionRoutingModule {}
