export enum Role {
  /**
   * @description
   * Assumed that if you're authenticated that you are
   * a USER unless you have the ADMIN role.
   *
   * @deprecated
   */
  PRACTITIONER = 'PRACTITIONER',
  MOA = 'MOA',
  ADMIN = 'ADMIN',
}
