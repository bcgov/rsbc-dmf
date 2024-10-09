import { APP_INITIALIZER, importProvidersFrom, Provider } from "@angular/core";
import { ConfigurationService } from '../../shared/services/configuration.service';
import { KeycloakAngularModule } from "keycloak-angular";
import { firstValueFrom, switchMap } from "rxjs";
import { ProfileManagementService } from "@app/shared/services/profile.service";
import { KeycloakInitService } from "@shared/core-ui";

export function keycloakFactory(configService: ConfigurationService, keycloakInitService: KeycloakInitService, profileManagementService: ProfileManagementService)
{
  return () => configService
    .load()
    .pipe(
      switchMap<any, any>(
        async (appConfiguration) => keycloakInitService.load(appConfiguration).then(async (isAuthenticated: boolean) => {
          if (isAuthenticated) {
            // Cache profile, await for it to complete so that we can guarantee the profile is loaded
            await firstValueFrom(profileManagementService.cacheProfile());
          }
        })
    ));
}

export function provideKeycloak(): Provider
{
  return [
    {
      provide: APP_INITIALIZER,
      useFactory: keycloakFactory,
      multi: true,
      deps: [ConfigurationService, KeycloakInitService, ProfileManagementService],
    },
    importProvidersFrom(KeycloakAngularModule)
  ]
}
