export enum IdentityProvider {
  BCSC = 'bcsc',
  IDIR = 'idir',
  PHSA = 'phsa',
}

export enum Role {
  Practitioner = 'PRACTITIONER',
  Enrolled = 'DMFT_ENROLLED'
}

export const RoleDescription = new Map<Role, string>([
  [Role.Practitioner, 'Practitioner'],
  [Role.Enrolled, 'Enrolled']
])
