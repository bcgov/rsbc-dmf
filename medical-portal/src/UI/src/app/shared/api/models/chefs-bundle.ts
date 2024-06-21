import { DriverInfoReply } from './driver-info-reply';
import { PatientCase } from './patient-case';

export interface ChefsBundle {
  caseId: string;
  patientCase: PatientCase;
  driverInfoReply: DriverInfoReply;
}
