import { Component } from '@angular/core';
import { LoginService } from './login.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {

  constructor(private loginService: LoginService) { }

  public showNavigation(): boolean {
    return this.loginService.isLoggedIn();
  }
}
