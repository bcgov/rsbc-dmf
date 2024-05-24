import { inject } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { from, lastValueFrom } from 'rxjs';

export const BearerTokenInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): any => {
  return from(handle(req, next));
};

async function handle(req: any, next: any) {
  const keycloakService = inject(KeycloakService);
  const bearerToken = await keycloakService.getToken();

  if (!req.headers.has('Authorization')) {
      req = req.clone({
          headers: req.headers.set('Authorization', `Bearer ${bearerToken}`)
      });
  }

  return lastValueFrom(next(req));
}
