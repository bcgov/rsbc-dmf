/* tslint:disable */
/* eslint-disable */
import { DriverMedicals } from '../models/driver-medicals';
import { DriverStatus } from '../models/driver-status';
export interface Driver {
  addressLine1?: string | null;
  birthDate?: string | null;
  city?: string | null;
  country?: string | null;
  driverLicenseNumber?: string | null;
  firstName?: string | null;
  flag51?: boolean | null;
  height?: string | null;
  id?: string | null;
  lastName?: string | null;
  licenceClass?: string | null;
  licenceExpiryDate?: string | null;
  loadedFromICBC?: boolean | null;
  masterStatusCode?: string | null;
  medicalIssueDate?: string | null;
  medicals?: Array<DriverMedicals> | null;
  postal?: string | null;
  province?: string | null;
  securityKeyword?: string | null;
  sex?: string | null;
  status?: Array<DriverStatus> | null;
  weight?: string | null;
}
