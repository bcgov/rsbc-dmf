/* tslint:disable */
/* eslint-disable */
import { ResultStatus } from '../models/result-status';
export interface DriverInfoReply {
  addressLine1?: string | null;
  birthDate?: string | null;
  city?: string | null;
  country?: string | null;
  errorDetail?: string | null;
  givenName?: string | null;
  postal?: string | null;
  province?: string | null;
  resultStatus?: ResultStatus;
  sex?: string | null;
  surname?: string | null;
}
