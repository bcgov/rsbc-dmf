import { Component, Inject, OnInit } from '@angular/core';
import { IsActiveMatchOptions } from '@angular/router';
import { Router } from '@angular/router';

import { Observable, map } from 'rxjs';

import { DashboardStateModel, PidpStateName } from '@pidp/data-model';
import { AppStateService } from '@pidp/presentation';

import {
  DashboardHeaderConfig,
  DashboardMenuItem,
  DashboardRouteMenuItem,
  IDashboard,
} from '@bcgov/shared/ui';
import { ArrayUtils } from '@bcgov/shared/utils';

import { APP_CONFIG, AppConfig } from '@app/app.config';
import { ContactService } from '@app/core/contact/contact.service';
import { AccessTokenService } from '@app/features/auth/services/access-token.service';
import { AuthService } from '@app/features/auth/services/auth.service';
import { CaseAssistanceRoutes } from '@app/features/case-assistance/case-assistance.routes';
import { CaseRoutes } from '@app/features/cases/case.routes';
import { EndorsementRoutes } from '@app/features/portal/endorsement/endorsement.routes';
//import { ProfileStatus } from '@app/features/portal/models/profile-status.model';
import { PortalResource } from '@app/features/portal/portal-resource.service';
import { PortalRoutes } from '@app/features/portal/portal.routes';
import { PermissionsService } from '@app/modules/permissions/permissions.service';
import { Role } from '@app/shared/enums/roles.enum';

@Component({
  selector: 'app-portal-dashboard',
  templateUrl: './portal-dashboard.component.html',
  styleUrls: ['./portal-dashboard.component.scss'],
})
export class PortalDashboardComponent implements IDashboard, OnInit {
  public logoutRedirectUrl: string;
  public username: Observable<string>;
  public headerConfig: DashboardHeaderConfig;
  public brandConfig: { imgSrc: string; imgAlt: string };
  public showMenuItemIcons: boolean;
  public firstname: Observable<string>;
  public lastname: Observable<string>;
  public responsiveMenuItems: boolean;
  public menuItems: DashboardMenuItem[];

  public dashboardState$ = this.stateService.stateBroadcast$.pipe(
    map((state) => {
      const dashboardNamedState = state.all.find(
        (x) => x.stateName === PidpStateName.dashboard
      );
      if (!dashboardNamedState) {
        throw 'dashboard state not found';
      }
      const dashboardState = dashboardNamedState as DashboardStateModel;
      return dashboardState;
    })
  );

  public constructor(
    @Inject(APP_CONFIG) private config: AppConfig,
    private authService: AuthService,
    private permissionsService: PermissionsService,
    accessTokenService: AccessTokenService,
    private portalResourceService: PortalResource,
    private contactService: ContactService,
    private stateService: AppStateService,
    private router: Router
  ) {
    this.logoutRedirectUrl = `${this.config.applicationUrl}/${this.config.routes.auth}`;
    this.username = accessTokenService.decodeToken().pipe(
      map((token) => {
        return token?.name ?? '';
      })
    );
    this.firstname = accessTokenService.decodeToken().pipe(
      map((token) => {
        return token?.given_name ?? '';
      })
    );
    this.lastname = accessTokenService.decodeToken().pipe(
      map((token) => {
        return token?.family_name ?? '';
      })
    );
    this.headerConfig = { theme: 'light', allowMobileToggle: true };
    this.brandConfig = {
      imgSrc: '/assets/images/dmft-portal-logo.svg',
      imgAlt: 'Driver Medical fitness Portal Logo',
    };
    this.showMenuItemIcons = true;
    this.responsiveMenuItems = false;
    this.menuItems = this.createMenuItems();
  }
  public ngOnInit(): void {
    // Get profile/Contact status
    // TODO: Insert a caching layer so a full get is not always required.
    const contactId = this.contactService.contactId;

    // Use forkJoin to wait for both to return.
    this.portalResourceService
      .getProfileStatus(contactId)
      .subscribe((profileStatus) => {
        const fullNameText = this.getUserFullNameText(profileStatus);
        const roleName = this.getContactRole(profileStatus);

        // Set the user name and college on the dashboard.
        const oldState = this.stateService.getNamedState<DashboardStateModel>(
          PidpStateName.dashboard
        );
        const newState: DashboardStateModel = {
          ...oldState,
          userProfileFullNameText: fullNameText,
          userProfileRoleNameText: roleName,
        };
        this.stateService.setNamedState(PidpStateName.dashboard, newState);
      });
  }

  public onLogout(): void {
    this.authService.logout(this.logoutRedirectUrl);
  }
  private createMenuItems(): DashboardMenuItem[] {
    const linkActiveOptions = {
      matrixParams: 'exact',
      queryParams: 'exact',
      paths: 'exact',
      fragment: 'exact',
    } as IsActiveMatchOptions;
    return [
      new DashboardRouteMenuItem(
        'Dashboard',
        {
          commands: PortalRoutes.MODULE_PATH,
          extras: { fragment: 'cases' },
          linkActiveOptions,
        },
        'assignment_ind'
      ),
      new DashboardRouteMenuItem(
        'Nearby Clinic',
        {
          commands: CaseRoutes.MODULE_PATH,
          extras: { fragment: 'clinic' },
          linkActiveOptions,
        },
        'assignment_ind'
      ),
      ...ArrayUtils.insertResultIf<DashboardRouteMenuItem>(
        this.permissionsService.hasRole([Role.ADMIN]),
        () => [
          new DashboardRouteMenuItem(
            'Administration',
            {
              commands: PortalRoutes.MODULE_PATH, //no admin page for now but good to have. An Admin page should ne required incase of troubleshooting
              extras: { fragment: 'administration' },
              linkActiveOptions,
            },
            'security'
          ),
        ]
      ),
      new DashboardRouteMenuItem(
        'View Endorsement',
        {
          commands: EndorsementRoutes.MODULE_PATH,
          extras: { fragment: 'endorsement' },
          linkActiveOptions,
        },
        'endorsement'
      ),
      new DashboardRouteMenuItem(
        'Get Support',
        {
          commands: CaseAssistanceRoutes.MODULE_PATH,
          extras: { fragment: 'support' },
          linkActiveOptions,
        },
        'help_outline'
      ),
    ];
  }
  private getUserFullNameText(profileStatus: any | null): string {
    if (profileStatus?.firstName) {
      const fullName = `${profileStatus?.firstName} ${profileStatus?.lastName}`;
      return fullName;
    } else {
      return '';
    }
  }
  private getContactRole(profileStatus: any | null): string {
    if (profileStatus?.role) {
      const roleName = `${profileStatus?.role}`;
      return roleName;
    } else {
      return '';
    }
  }
}
