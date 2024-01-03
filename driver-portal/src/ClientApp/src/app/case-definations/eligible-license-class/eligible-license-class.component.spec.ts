import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EligibleLicenseClassComponent } from './eligible-license-class.component';

describe('EligibleLicenseClassComponent', () => {
  let component: EligibleLicenseClassComponent;
  let fixture: ComponentFixture<EligibleLicenseClassComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EligibleLicenseClassComponent]
    });
    fixture = TestBed.createComponent(EligibleLicenseClassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
