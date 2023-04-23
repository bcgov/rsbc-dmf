/* eslint-disable @typescript-eslint/no-explicit-any */
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { randAbn, randAlphaNumeric, randNumber } from '@ngneat/falso';
import { Spy, createSpyFromClass, provideAutoSpy } from 'jest-auto-spies';

import { RootRoutes } from '@bcgov/shared/ui';

import { APP_CONFIG } from '@app/app.config';

import { DocumentService } from '../services/document.service';
import { LoggerService } from '../services/logger.service';
import { ContactResource } from './contact-resource.service';
import { ContactResolver } from './contact.resolver';
import { ContactService } from './contact.service';

describe('ContactResolver', () => {
  let resolver: ContactResolver;
  let contactResourceSpy: Spy<ContactResource>;
  let contactServiceSpy: Spy<ContactService>;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        ContactResolver,
        {
          provide: APP_CONFIG,
          useValue: APP_DI_CONFIG,
        },
        {
          provide: ContactService,
          useValue: createSpyFromClass(ContactService, {
            gettersToSpyOn: ['contactId'],
            settersToSpyOn: ['contactId'],
          }),
        },
        provideAutoSpy(ContactResource),
        provideAutoSpy(LoggerService),
        provideAutoSpy(DocumentService),
        provideAutoSpy(Router),
      ],
    });

    router = TestBed.inject(Router);
    resolver = TestBed.inject(ContactResolver);
    contactResourceSpy = TestBed.inject<any>(ContactResource);
    contactServiceSpy = TestBed.inject<any>(ContactService);
  });

  describe('METHOD: resolve', () => {
    given('a contact ID does not exist', () => {
      contactServiceSpy.accessorSpies.setters.contactId(null);

      when('attempting to resolve the contact is successful', () => {
        const contactId = randAbn();
        contactResourceSpy.firstOrCreate.nextOneTimeWith(contactId);
        let actualResult: string | null;
        resolver
          .resolve()
          .subscribe((contactId: string | null) => (actualResult = contactId));

        then('response will provide the contact ID', () => {
          expect(contactResourceSpy.firstOrCreate).toHaveBeenCalledTimes(1);
          expect(actualResult).toBe(contactId);
        });
      });
    });

    given('a contact ID does not exist', () => {
      contactServiceSpy.accessorSpies.setters.contactId(null);

      when('attempting to resolve the contact is unsuccessful', () => {
        contactResourceSpy.firstOrCreate.nextWithValues([
          {
            errorValue: new Error('Anonymous error of any type'),
          },
        ]);
        let actualResult: string | null;
        resolver
          .resolve()
          .subscribe((contactId: string | null) => (actualResult = contactId));

        then('response will provide the contact ID', () => {
          expect(contactResourceSpy.firstOrCreate).toHaveBeenCalledTimes(1);
          expect(router.navigateByUrl).toHaveBeenCalledWith(RootRoutes.DENIED);
          expect(actualResult).toBe(null);
        });
      });
    });
  });
});
