/* tslint:disable */
/* eslint-disable */
import { FullAddress } from '../models/full-address';
export interface UserRegistration {
  address?: FullAddress;
  driverLicenseNumber?: string | null;
  email?: string | null;
  notifyByEmail?: boolean;
  notifyByMail?: boolean;
}
