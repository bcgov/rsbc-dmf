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
    return this.profileService.apiProfileCurrentGet$Json({...params}).pipe(map((res) => ({...res, businessName: 'Dr. Shelby Drew', licensingInstitution:"College of physicians and surgeons", licenseNumber:"12344555"})));
  }

  public updateProfile(params: Parameters<ProfileService['apiProfileEmailPut']>[0]) {
    return this.profileService.apiProfileEmailPut({...params});
  }

  public getProfilePractitionerRoles(params: Parameters<ProfileService['apiProfilePractitionerRolesGet$Response']>[0]) {
    return this.profileService.apiProfilePractitionerRolesGet$Response({...params}).pipe(map((res) => res.body));
  }

  public updateProfilePractitionerRoles(params: Parameters<ProfileService['apiProfilePractitionerRolesPut$Response']>[0]) {
    return this.profileService.apiProfilePractitionerRolesPut$Response({...params}).pipe(map((res) => res.body));;
  }

  public deleteProfilePractitionerRoles(params: Parameters<ProfileService['apiProfilePractitionerRolesDelete$Response']>[0]) {
    return this.profileService.apiProfilePractitionerRolesDelete$Response({...params}).pipe(map((res) => res.body));;
  }

}
