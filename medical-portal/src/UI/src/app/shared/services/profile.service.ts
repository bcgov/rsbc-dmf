import { Injectable } from '@angular/core';
import { ProfileService } from '../api/services';
import { UserProfile } from '../api/models';
import { Observable, catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfileManagementService {
  constructor(private profileService: ProfileService) { }

  public getProfile(): Observable<UserProfile> {
    return this.profileService.apiProfileCurrentGet$Json();
  }

  public getCachedProfile(): UserProfile {
    let profile = sessionStorage.getItem('profile');
    if (!profile) return {};
    return JSON.parse(profile) as UserProfile;
  }
  
  public cacheProfile(): Observable<UserProfile> {
    let profile = sessionStorage.getItem('profile');

    if (profile === null || undefined) {
      return this.getProfile().pipe(
        map((profile) => {
          sessionStorage.setItem('profile', JSON.stringify(profile));
          return profile;
        }),
        catchError((error) => { console.log(error);         return of({});}
      ));
    } else {
      if (!profile) return of({});
      return of(JSON.parse(profile) as UserProfile);
    }
  }
}
