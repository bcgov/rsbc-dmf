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
export class ContactService {
  private _contactId!: string;

  /**
   * @description
   * Single use setter for the Contact identifier, which
   * should only be used by the ContactResolver.
   */
  public set contactId(contactId: string) {
    if (!this._contactId) {
      this._contactId = contactId;
    }
  }

  public get contactId(): string {
    return this._contactId;
  }
}
