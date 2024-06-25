/* tslint:disable */
/* eslint-disable */
import { Driver } from '../models/driver';
import { PatientCase } from '../models/patient-case';
export interface ChefsBundle {
  caseId?: string | null;
  driverInfo?: Driver;
  patientCase?: PatientCase;
}
