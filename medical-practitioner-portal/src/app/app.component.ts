import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Layout/header/header.component';
import { FooterComponent } from './Layout/footer/footer.component';
import { NavMenuComponent } from './Layout/nav-menu/nav-menu.component';
import { firstValueFrom } from 'rxjs';
import { LoginService } from './shared/services/login.service';

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

  constructor(private loginService: LoginService) { }

  public async ngOnInit(): Promise<void> {
    try {
      //attempt to log in
      let nextRoute = await firstValueFrom(this.loginService.login(location.pathname.substring(1) || 'dashboard'));

      //get the user's profile
      await firstValueFrom(this.loginService.getUserProfile());
    } catch (e) {
      console.error(e);
      throw e;
    }
  }
}
