import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerPortalHeaderComponent } from './partner-portal-header.component';

describe('PartnerPortalHeaderComponent', () => {
  let component: PartnerPortalHeaderComponent;
  let fixture: ComponentFixture<PartnerPortalHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PartnerPortalHeaderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PartnerPortalHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
