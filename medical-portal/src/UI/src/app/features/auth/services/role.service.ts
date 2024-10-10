import { Injectable } from "@angular/core";
import { Role } from "../enums/role.enum";
import { KeycloakService } from "keycloak-angular";

@Injectable({
  providedIn: 'root',
})
export class RoleService
{
  constructor(private keycloakService: KeycloakService) { }

  // TODO get string[] from shared/core-ui and remove KeycloakService dependency
  public getRoles(): Role[] {
    const roleNames = this.keycloakService
      .getUserRoles()
      .filter((role) => role !== Role.Enrolled);
    return roleNames
      .map((role) => Object.values(Role).find((value) => value === role))
      .filter((role) => role !== undefined) as Role[];
  }
}
