import { CUSTOM_ELEMENTS_SCHEMA, Component, Optional } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Layout/header/header.component';
import { FooterComponent } from './Layout/footer/footer.component';
import { NavMenuComponent } from './Layout/nav-menu/nav-menu.component';
import { AuthService } from './features/auth/services/auth.service';
import { IdentityProvider } from './features/auth/enums/identity-provider.enum';
import { ApplicationVersionInfoService } from './shared/api/services';
import { ApplicationVersionInfo } from './shared/api/models';
import { firstValueFrom } from 'rxjs';
import { NgIf } from '@angular/common';
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    NavMenuComponent,
    NgIf,
    NgxSpinnerComponent,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  //exports : [],
  //declarations :[CoreUiModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'medical-practitioner-portal';
  //public versionInfo: ApplicationVersionInfo | null = null;
  public isLoading = true;
  public profileName: string = '';

  constructor(
    private authService: AuthService,
    @Optional() private versionInfoDataService: ApplicationVersionInfoService,
  ) {}

  public async ngOnInit(): Promise<void> {
    try {
      console.info('AppComponent initializing...');
      //attempt to log in
      this.authService.isLoggedIn().subscribe((isLoggedIn) => {
        console.info('Are you logged in?', isLoggedIn);
        if (!isLoggedIn) {
          console.info('Redirect to login page');
          this.authService.login({
            idpHint: IdentityProvider.BCSC,
            // TODO add medical-portal scope and move this to api/Config
            scope: 'openid profile email',
          });
        } else {
          // for spinner status, this will likely change when the keycloak auth lifecycle events are refactored
          this.isLoading = false;
        }
      });
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      console.info('AppComponent initialization completed.');
    }
  }

  //  Logout
  public onLogout(): void {
    this.authService.logout;
  }
}
