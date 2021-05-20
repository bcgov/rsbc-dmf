import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { LoginService } from '../login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {


  public loginForm = this.fb.group({
    userName: [null, Validators.required],
    password: [null, Validators.required],
  });

  constructor(private fb: FormBuilder, private loginService: LoginService) { }

  onSubmit(): void {
    console.log('submit');
    this.loginService.setLoggedIn();
  }
}
