import { User } from '@app/features/auth/models/user.model';

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface ContactCreate
  extends Pick<User, 'userId' | 'firstName' | 'lastName'> {}
