import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SetDashboardTitleGuard } from '@pidp/presentation';

import { PortalPage } from './portal.page';

const routes: Routes = [
  {
    path: '',
    component: PortalPage,
    canActivate: [SetDashboardTitleGuard],
    data: {
      title: 'Medical Practitioner Portal',
      setDashboardTitleGuard: {
        titleText: 'Medical Practitioner Portal',
        titleDescriptionText: '',
      },
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PortalRoutingModule {}
