import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Layout/header/header.component';
import { FooterComponent } from './Layout/footer/footer.component';
import { NavMenuComponent } from './Layout/nav-menu/nav-menu.component';
import { AuthService } from './features/auth/services/auth.service';
import { IdentityProvider } from './features/auth/enums/identity-provider.enum';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, NavMenuComponent ],
  //exports : [],
  //declarations :[CoreUiModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})


export class AppComponent {
  title = 'medical-practitioner-portal';

  constructor(private authService: AuthService) { }

  public async ngOnInit(): Promise<void> {
    try {
      //attempt to log in
      this.authService.isLoggedIn().subscribe((isLoggedIn) => {
        console.log("isLoggedIn", isLoggedIn);
        if (!isLoggedIn) {
          this.authService.login({
            idpHint: IdentityProvider.BCSC
          })
        }
    });

    } catch (e) {
      console.error(e);
      throw e;
    }
  }
}
