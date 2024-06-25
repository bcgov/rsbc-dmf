/* tslint:disable */
/* eslint-disable */
import { DriverInfoReply } from '../models/driver-info-reply';
import { PatientCase } from '../models/patient-case';
export interface ChefsBundle {
  caseId?: string | null;
  driverInfoReply?: DriverInfoReply;
  patientCase?: PatientCase;
}
