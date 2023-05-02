import { TestBed } from '@angular/core/testing';

import { EndorsementResourceService } from './endorsement-resource.service';

describe('EndorsementResourceService', () => {
  let service: EndorsementResourceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EndorsementResourceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
