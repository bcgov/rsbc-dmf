import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerPortalFooterComponent } from './partner-portal-footer.component';

describe('PartnerPortalFooterComponent', () => {
  let component: PartnerPortalFooterComponent;
  let fixture: ComponentFixture<PartnerPortalFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PartnerPortalFooterComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PartnerPortalFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
