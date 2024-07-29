import { Injectable } from '@angular/core';
import { LoginService } from './login.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private loginService: LoginService) { }

  getUserId(): string {
    return this.loginService.userProfile?.id as string;
  }
}
