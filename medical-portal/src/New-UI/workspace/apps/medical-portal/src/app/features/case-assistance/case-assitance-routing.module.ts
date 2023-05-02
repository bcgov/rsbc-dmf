import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SetDashboardTitleGuard } from '@pidp/presentation';

import { CaseAssistanceComponent } from './case-assistance.component';

const routes: Routes = [
  {
    path: '',
    component: CaseAssistanceComponent,
    canActivate: [SetDashboardTitleGuard],
    data: {
      title: 'Case Assistance',
      setDashboardTitleGuard: {
        titleText: 'Case Assistance',
        titleDescriptionText: '',
      },
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CaseAssistanceRoutingModule {}
