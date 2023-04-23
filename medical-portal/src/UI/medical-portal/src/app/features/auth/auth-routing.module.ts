import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthRoutes } from './auth.routes';
import { AuthorizationRedirectGuard } from './guards/authorization-redirect.guard';
import { LoginPage } from './pages/login/login.page';

const routes: Routes = [
  {
    path: AuthRoutes.PORTAL_LOGIN,
    canActivate: [AuthorizationRedirectGuard],
    component: LoginPage,
    data: {
      loginPageData: {
        isAdminLogin: false,
      },
      setDashboardTitleGuard: {
        titleText: '',
        titleDescriptionText: '',
      },
    },
  },
  {
    path: AuthRoutes.ADMIN_LOGIN,
    canActivate: [AuthorizationRedirectGuard],
    component: LoginPage,
    data: {
      loginPageData: {
        title: 'Provider Identity Portal',
        isAdminLogin: true,
      },
      routes: {
        auth: AuthRoutes.routePath(AuthRoutes.ADMIN_LOGIN),
      },
    },
  },
  {
    path: '',
    redirectTo: AuthRoutes.PORTAL_LOGIN,
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
    schemas: [
    // This causes the compiler to allow the non-angular swiper html tags.
    // Without this schema, compiling will fail on the swiper tags.
    CUSTOM_ELEMENTS_SCHEMA,
  ],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
