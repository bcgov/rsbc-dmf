export enum Role {
  Practitioner = 'PRACTITIONER',
  Enrolled = 'DMFT_ENROLLED',
  Moa = 'MOA',
}

export const RoleDescription = new Map<Role, string>([
  [Role.Practitioner, 'Practitioner'],
  [Role.Enrolled, 'Enrolled'],
  [Role.Moa, 'MOA']
])
