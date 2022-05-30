import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ProfileService } from '../api/services';

@Injectable({
  providedIn: 'root'
})
export class ProfileManagementService {
  constructor(private profileService: ProfileService) { }

  public getProfile(params: Parameters<ProfileService['apiProfileCurrentGet$Json']>[0]) {
    return this.profileService.apiProfileCurrentGet$Json({...params});
  }

  public updateProfile(params: Parameters<ProfileService['apiProfileEmailPut']>[0]) {
    return this.profileService.apiProfileEmailPut({...params});
  }
}
