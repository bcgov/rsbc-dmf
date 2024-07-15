/* tslint:disable */
/* eslint-disable */
import { MedicalConditionItem } from '../models/medical-condition-item';
import { Timestamp } from '../models/timestamp';
export interface CaseDetail {
  assigneeTitle?: string | null;
  birthDate?: Timestamp;
  caseId?: string | null;
  caseSequence?: number;
  caseType?: string | null;
  decisionDate?: Timestamp;
  decisionForClass?: string | null;
  dmerType?: string | null;
  dpsProcessingDate?: Timestamp;
  driverId?: string | null;
  driverLicenseNumber?: string | null;
  eligibleLicenseClass?: string | null;
  firstName?: string | null;
  idCode?: string | null;
  lastActivityDate?: Timestamp;
  lastName?: string | null;
  latestComplianceDate?: Timestamp;
  latestDecision?: string | null;
  medicalConditions?: Array<MedicalConditionItem> | null;
  middlename?: string | null;
  name?: string | null;
  openedDate?: Timestamp;
  outstandingDocuments?: number;
  status?: string | null;
  title?: string | null;
}
