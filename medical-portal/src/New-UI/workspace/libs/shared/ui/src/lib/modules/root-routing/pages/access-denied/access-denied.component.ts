import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { RouteUtils } from '@bcgov/shared/utils';

import { APP_CONFIG, AppConfig } from '@app/app.config';

@Component({
  selector: 'ui-access-denied',
  template: `
    <ui-root-route-container>
      <div class="row justify-content-center">
        <div class="col-sm-12 col-md-10 col-lg-8 text-center">
          <h1 class="mb-5">
            403
            <span class="d-block d-lg-inline">
              You don't appear to have the proper authorization.
            </span>
          </h1>

          <button mat-flat-button (click)="routeToRoot()">
            Please, go to the Health Provider Identity Portal to Enrol for
            Access!
          </button>
        </div>
      </div>
    </ui-root-route-container>
  `,
  styleUrls: ['../../shared/root-route-page-styles.scss'],
})
export class AccessDeniedComponent {
  private uri: string;
  public constructor(
    @Inject(APP_CONFIG) config: AppConfig,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.uri = config.pidpPortalUrl;
  }

  public routeToRoot(): void {
    this.router.navigate(['/']).then((result) => {
      window.location.href = this.uri;
    });
  }
}
