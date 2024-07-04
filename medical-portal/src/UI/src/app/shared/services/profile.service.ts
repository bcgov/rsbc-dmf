import { Injectable } from '@angular/core';
import { ProfileService } from '../api/services';
import { UserProfile } from '../api/models';
import { Observable, catchError, map, of } from 'rxjs';
import { SESSION_STORAGE_KEYS } from '@app/app.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileManagementService {
  constructor(private profileService: ProfileService) { }

  // get profile from Api
  public getProfile(): Observable<UserProfile> {
    return this.profileService.apiProfileCurrentGet$Json();
  }

  // get profile from Cache
  public getCachedProfile(): UserProfile {
    // TODO abstract sessionStorage with a "local storage service", in case we want to replace with another storage mechanism
    // TODO move the SESSION_STORAGE_KEYS to the above service
    let profile = sessionStorage.getItem(SESSION_STORAGE_KEYS.PROFILE);
    if (!profile) return {};
    return JSON.parse(profile) as UserProfile;
  }

  // get profile from Api and cache it
  public cacheProfile(): Observable<UserProfile> {
    return this.getProfile().pipe(
      map((profile) => {
        sessionStorage.setItem(SESSION_STORAGE_KEYS.PROFILE, JSON.stringify(profile));
        return profile;
      }),
      catchError((error) => {
        console.error(error);
        return of({});
      }
    ));
  }
}
