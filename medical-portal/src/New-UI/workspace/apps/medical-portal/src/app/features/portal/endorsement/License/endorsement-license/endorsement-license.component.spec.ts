import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EndorsementLicenseComponent } from './endorsement-license.component';

describe('EndorsementLicenseComponent', () => {
  let component: EndorsementLicenseComponent;
  let fixture: ComponentFixture<EndorsementLicenseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EndorsementLicenseComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EndorsementLicenseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
