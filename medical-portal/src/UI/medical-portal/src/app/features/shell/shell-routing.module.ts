import { PortalModule } from '@angular/cdk/portal';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContactResolver } from '@app/core/contact/contact.resolver';
import { DashboardComponent } from '@app/dashboard/dashboard.component';
import { PermissionsGuard } from '@app/modules/permissions/permissions.guard';
import { Role } from '@app/shared/enums/roles.enum';

import { AuthModule } from '../auth/auth.module';
import { AuthRoutes } from '../auth/auth.routes';
import { AuthenticationGuard } from '../auth/guards/authentication.guard';
import { SupportErrorModule } from '../shell/pages/support-error/support-error.module';
//import { PortalDashboardComponent } from './components/portal-dashboard/portal-dashboard.component';
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
    component: DashboardComponent,
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
        path: '',
        //redirectTo: PortalRoutes.MODULE_PATH,
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
