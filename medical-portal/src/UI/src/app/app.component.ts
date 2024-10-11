import { CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MedicalHeaderComponent } from './Layout/medical-header/medical-header.component';
import { MedicalFooterComponent } from './Layout/medical-footer/medical-footer.component';
import { MedicalNavMenuComponent } from './Layout/medical-nav-menu/medical-nav-menu.component';
import { AuthService } from '@shared/core-ui';
import { NgIf } from '@angular/common';
import { NgxSpinnerComponent } from 'ngx-spinner';

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
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'medical-practitioner-portal';
  public isLoading = true;
  public profileName: string = '';

  constructor(
    private authService: AuthService
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
