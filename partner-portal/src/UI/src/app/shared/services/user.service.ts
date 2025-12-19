import { Injectable } from '@angular/core';
import { DriverService } from '../api/services';
import { Driver, UserContext } from '../api/models';
import { Observable } from 'rxjs';
import { SESSION_STORAGE_KEYS } from '@app/app.model';
import {
  UserService
  //ProfileService,
} from '../api/services';
//import { ProfileService } from '../api/services';

@Injectable({
  providedIn: 'root'
})
export class UserServices {
  constructor(private driverService: DriverService, private userService: UserService) { }

  getUserId(): string {
    return this.getCachedriver().id as string;
  }

  getDriverSession(): Observable<UserContext> {
    return this.driverService.apiDriverDriverSessionGet$Json();
  }

  getCachedriver(): Driver {
    let driver = sessionStorage.getItem(SESSION_STORAGE_KEYS.DRIVER);
    if (!driver) return {};
    return JSON.parse(driver) as Driver;
  }

  // Set cache driver on driver serach
  setCacheDriver(driver: Driver) {
    sessionStorage.setItem(SESSION_STORAGE_KEYS.DRIVER, JSON.stringify(driver));
  }

  public getCurrentLoginDetails(): Observable<string[]>{
    return this.userService.apiUserGetCurrentLoginDetailsGet$Json();
  }

}
