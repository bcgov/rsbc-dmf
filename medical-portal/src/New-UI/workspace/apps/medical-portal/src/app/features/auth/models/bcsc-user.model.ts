import { IdentityProvider } from '../enums/identity-provider.enum';
import { UserIdentity } from './user-identity.model';
import { IUserResolver, User } from './user.model';

export class BcscUser implements User {
  public readonly identityProvider: IdentityProvider;
  public userId: string;
  public idpId: string;
  public firstName: string;
  public lastName: string;
  public email?: string | undefined;
  public pidp_email?: string;
  public birthdate: string;
  public gender?: string;
  public roles: string[];

  public constructor({ accessTokenParsed }: UserIdentity) {
    const {
      identity_provider,
      sub: userId,
      resource_access: KeycloakRoles,
      family_name: lastName,
      given_name: firstName,
      preferred_username: idpId,
      email: email,
      pidp_email: pidp_email,
      birthdate: birthDate,
    } = accessTokenParsed;

    this.identityProvider = identity_provider;
    this.userId = userId;
    this.idpId = idpId;
    this.firstName = firstName;
    this.lastName = lastName;
    this.pidp_email = pidp_email;
    this.email = email ? email : `${firstName}.${lastName}@abc.com`;
    this.birthdate = birthDate;
    this.roles = Object.values(KeycloakRoles || {}).flatMap(
      (clientRoles: { roles: any }) => clientRoles.roles
    );
  }
}

export class BcscResolver implements IUserResolver<BcscUser> {
  public constructor(public userIdentity: UserIdentity) {}
  public resolve(): BcscUser {
    return new BcscUser(this.userIdentity);
  }
}
