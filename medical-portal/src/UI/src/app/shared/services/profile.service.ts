import { Injectable } from '@angular/core';
import { ProfileService } from '../api/services';

@Injectable({
  providedIn: 'root'
})
export class ProfileManagementService {
  constructor(private profileService: ProfileService) { }

  public getProfile() {
    return this.profileService.apiProfileCurrentGet$Json();
  }
}
