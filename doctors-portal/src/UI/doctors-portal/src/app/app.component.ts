import { Component } from '@angular/core';
import { LoginService } from './login.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {

  public title = 'RSBC - DFT - Doctor\'s portal';

  constructor(private loginService: LoginService) { }

  public showNavigation(): boolean {
    console.debug('showNav', this.loginService.isLoggedIn());
    return this.loginService.isLoggedIn();
  }
}
