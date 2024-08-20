import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerPortalNavMenuComponent } from './partner-portal-nav-menu.component';

describe('PartnerPortalNavMenuComponent', () => {
  let component: PartnerPortalNavMenuComponent;
  let fixture: ComponentFixture<PartnerPortalNavMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PartnerPortalNavMenuComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PartnerPortalNavMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
