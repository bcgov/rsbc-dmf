import { CUSTOM_ELEMENTS_SCHEMA, Component, Optional } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MedicalHeaderComponent } from './Layout/medical-header/medical-header.component';
import { MedicalFooterComponent } from './Layout/medical-footer/medical-footer.component';
import { MedicalNavMenuComponent } from './Layout/medical-nav-menu/medical-nav-menu.component';
import { AuthService } from './features/auth/services/auth.service';
import { IdentityProvider } from './features/auth/enums/identity-provider.enum';
import { ApplicationVersionInfoService } from './shared/api/services';
import { ApplicationVersionInfo } from './shared/api/models';
import { firstValueFrom } from 'rxjs';
import { NgIf } from '@angular/common';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { ConfigurationService } from './shared/services/configuration.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    MedicalHeaderComponent,
    MedicalFooterComponent,
    MedicalNavMenuComponent,
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
    private configService: ConfigurationService,
    @Optional() private versionInfoDataService: ApplicationVersionInfoService,
  ) {}

  public async ngOnInit(): Promise<void> {
    this.isLoading = false;

    console.info('AppComponent initialization completed.');
  }

  //  Logout
  public onLogout(): void {
    this.authService.logout;
  }
}
