/* tslint:disable */
/* eslint-disable */
import { Driver } from '../models/driver';
import { MedicalCondition } from '../models/medical-condition';
import { PatientCase } from '../models/patient-case';
export interface ChefsBundle {
  caseId?: string | null;
  driverInfo?: Driver;
  medicalConditions?: Array<MedicalCondition> | null;
  patientCase?: PatientCase;
}
