/* tslint:disable */
/* eslint-disable */
import { Address } from '../models/address';
export interface Driver {
  address?: Address;
  birthDate?: string | null;
  driverLicenceNumber?: string | null;
  givenName?: string | null;
  height?: number;
  licenceClass?: number;
  name?: string | null;
  sex?: string | null;
  surname?: string | null;
  weight?: number;
}
