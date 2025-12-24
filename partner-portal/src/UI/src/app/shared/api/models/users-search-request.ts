/* tslint:disable */
/* eslint-disable */
import { UserType } from '../models/user-type';
export interface UsersSearchRequest {
  activeUser?: number;
  externalSystem?: string | null;
  externalSystemUserId?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  unauthorizedOnly?: boolean;
  userId?: string | null;
  userType?: UserType;
}
