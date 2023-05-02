import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SetDashboardTitleGuard } from '@pidp/presentation';

import { EndorsementComponentComponent } from './endorsement-component.component';

const routes: Routes = [
  {
    path: '',
    component: EndorsementComponentComponent,
    canActivate: [SetDashboardTitleGuard],
    data: {
      title: 'Practitioner Endorsement',
      setDashboardTitleGuard: {
        titleText: 'Practitioner Endorsement',
        titleDescriptionText:
          'Use this section to view and manage your endorsements',
      },
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EndorsementRoutingModuleModule {}
