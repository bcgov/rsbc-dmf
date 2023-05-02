import { TestBed } from '@angular/core/testing';

import { EndorsementService } from './endorsement.service';

describe('EndorsementService', () => {
  let service: EndorsementService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EndorsementService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
