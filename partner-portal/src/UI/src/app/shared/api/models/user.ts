/* tslint:disable */
/* eslint-disable */
import { AuditDetail } from '../models/audit-detail';
import { UserRole } from '../models/user-role';
export interface User {
  active?: boolean;
  addressLine1?: string | null;
  addressLine2?: string | null;
  addressLine3?: string | null;
  auditDetails?: Array<AuditDetail> | null;
  authorized?: boolean;
  cellNumber?: string | null;
  city?: string | null;
  country?: string | null;
  dfWebuserId?: string | null;
  domain?: string | null;
  effectiveDate?: string | null;
  email?: string | null;
  expiryDate?: string | null;
  firstName?: string | null;
  id?: string | null;
  lastName?: string | null;
  modifiedUserId?: string | null;
  phoneNumber?: string | null;
  postCode?: string | null;
  province?: string | null;
  roles?: Array<UserRole> | null;
  secondGivenName?: string | null;
  thirdGivenName?: string | null;
  userName?: string | null;
}
