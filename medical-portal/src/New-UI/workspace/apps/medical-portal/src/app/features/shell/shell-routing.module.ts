import { PortalModule } from '@angular/cdk/portal';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContactResolver } from '@app/core/contact/contact.resolver';
import { PermissionsGuard } from '@app/modules/permissions/permissions.guard';
import { Role } from '@app/shared/enums/roles.enum';

import { AuthModule } from '../auth/auth.module';
import { AuthRoutes } from '../auth/auth.routes';
import { AuthenticationGuard } from '../auth/guards/authentication.guard';
import { CaseAssistanceRoutes } from '../case-assistance/case-assistance.routes';
import { CaseRoutes } from '../cases/case.routes';
import { DmerSubmissionRoutes } from '../dmer-submission-confirmation/dmer-submission-confirmation.routes';
import { EndorsementRoutes } from '../portal/endorsement/endorsement.routes';
import { PortalRoutes } from '../portal/portal.routes';
import { SupportErrorModule } from '../shell/pages/support-error/support-error.module';
import { PortalDashboardComponent } from './components/portal-dashboard/portal-dashboard.component';
import { ShellRoutes } from './shell.routes';

const routes: Routes = [
  {
    path: AuthRoutes.MODULE_PATH,
    loadChildren: (): Promise<AuthModule> =>
      import('../auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: ShellRoutes.SUPPORT_ERROR_PAGE,
    loadChildren: (): Promise<SupportErrorModule> =>
      import('../shell/pages/support-error/support-error.module').then(
        (m) => m.SupportErrorModule
      ),
  },
  {
    path: '',
    component: PortalDashboardComponent,
    canActivate: [AuthenticationGuard],
    canActivateChild: [AuthenticationGuard],
    resolve: {
      contactId: ContactResolver,
    },
    data: {
      routes: {
        auth: `/${AuthRoutes.MODULE_PATH}`,
      },
    },
    children: [
      {
        path: PortalRoutes.MODULE_PATH,
        canActivate: [PermissionsGuard],
        data: {
          roles: [Role.PRACTITIONER, Role.MOA, Role.ENROLLED],
        },
        loadChildren: (): Promise<PortalModule> =>
          import('../portal/portal.module').then((m) => m.PortalModule),
      },
      {
        path: EndorsementRoutes.MODULE_PATH,
        loadChildren: (): Promise<PortalModule> =>
          import(
            '../portal/endorsement/endorsement-routing-module.module'
          ).then((m) => m.EndorsementRoutingModuleModule),
      },
      {
        path: CaseRoutes.MODULE_PATH, //create a route module for case later
        loadChildren: (): Promise<AuthModule> =>
          import('../cases/cases.module').then((m) => m.CasesModule),
      },
      {
        path: 'notifications', //create a route routing module later
        loadChildren: (): Promise<AuthModule> =>
          import('../notifications/notifications.module').then(
            (m) => m.NotificationsModule
          ),
      },
      {
        path: CaseAssistanceRoutes.MODULE_PATH,
        loadChildren: (): Promise<PortalModule> =>
          import('../case-assistance/case-assistance.module').then(
            (m) => m.CaseAssistanceModule
          ),
      },
      {
        path: DmerSubmissionRoutes.MODULE_PATH,
        loadChildren: (): Promise<PortalModule> =>
          import(
            '../dmer-submission-confirmation/dmer-submission-confirmation.module'
          ).then((m) => m.DmerSubmissionConfirmationModule),
      },
      {
        path: DmerSubmissionRoutes.MODULE_PATH,
        loadChildren: (): Promise<PortalModule> =>
          import(
            '../dmer-submission-confirmation/dmer-submission-confirmation.module'
          ).then((m) => m.DmerSubmissionConfirmationModule),
      },
      {
        path: '',
        redirectTo: PortalRoutes.MODULE_PATH,
        pathMatch: 'full',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ShellRoutingModule {}
