import { Injectable } from '@angular/core';

/**
 * @description
 * ContactService used to store Contact specific information
 * requested when a user routes into an authenticated route
 * configuration for use within the application.
 *
 * WARNING: Depends on the use of the ContactResolver!
 */
@Injectable({
  providedIn: 'root',
})
export class EndorsementService {
  private _hpdid!: string;

  /**
   * @description
   * Single use setter for the Contact identifier, which
   * should only be used by the ContactResolver.
   */
  public set hpdid(hpdid: string) {
    if (!this._hpdid) {
      this._hpdid = hpdid;
    }
  }

  public get hpdid(): string {
    return this._hpdid;
  }
}
