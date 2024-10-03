import { inject } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { from, lastValueFrom } from 'rxjs';

export const BearerTokenInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): any => {
  return from(handle(req, next));
};

async function handle(req: HttpRequest<any>, next: HttpHandlerFn) {
  const keycloakService = inject(KeycloakService);
  let bearerToken: string;

  try {
    bearerToken = await keycloakService.getToken();
  } catch (error) {
    // if keycloak is not initialized, getToken will error
    return next(req);
    return lastValueFrom(next(req));
  }

  if (!req.headers.has('Authorization')) {
      req = req.clone({
          headers: req.headers.set('Authorization', `Bearer ${bearerToken}`)
      });
  }

  return lastValueFrom(next(req));
}
