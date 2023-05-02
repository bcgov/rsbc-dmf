import { TestBed } from '@angular/core/testing';

import { provideAutoSpy } from 'jest-auto-spies';

import { ResourceUtilsService } from '@bcgov/shared/data-access';

import { AuthorizedUserService } from '@app/features/auth/services/authorized-user.service';

import { ApiHttpClient } from '../resources/api-http-client.service';
import { ContactResource } from './contact-resource.service';

describe('ContactResource', () => {
  let service: ContactResource;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ContactResource,
        provideAutoSpy(ApiHttpClient),
        provideAutoSpy(ResourceUtilsService),
        provideAutoSpy(AuthorizedUserService),
      ],
    });

    service = TestBed.inject(ContactResource);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
