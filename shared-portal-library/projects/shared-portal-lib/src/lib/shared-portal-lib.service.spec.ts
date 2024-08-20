import { TestBed } from '@angular/core/testing';

import { SharedPortalLibService } from './shared-portal-lib.service';

describe('SharedPortalLibService', () => {
  let service: SharedPortalLibService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SharedPortalLibService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
