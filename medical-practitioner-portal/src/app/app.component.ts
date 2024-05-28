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

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    NavMenuComponent,
    NgIf,
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
    @Optional() private versionInfoDataService: ApplicationVersionInfoService
  ) {}

  public async ngOnInit(): Promise<void> {
    try {
      // Load Configuration from server
      // await firstValueFrom(this.authService.)

      //attempt to log in
      // let nextRoute = await firstValueFrom(
      //   this.authService.login(location.pathname.substring(1) || 'dashboard')
      // );

      //attempt to log in
      this.authService.isLoggedIn().subscribe((isLoggedIn) => {
        if (!isLoggedIn) {
          this.authService.login({
            idpHint: IdentityProvider.BCSC,
          });
        }

        // Get Version info on footer
      });
    } catch (e) {
      console.error(e);
      throw e;
    }
  }

  //  Logout
  public onLogout(): void {
    this.authService.logout;
  }
}
