/* tslint:disable */
/* eslint-disable */
import { PreferredTime } from '../models/preferred-time';
export interface CallbackRequest {
  phone?: string | null;
  preferredTime?: PreferredTime;
  subject?: string | null;
}
