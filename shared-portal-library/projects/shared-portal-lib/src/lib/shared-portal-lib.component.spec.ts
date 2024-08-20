import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedPortalLibComponent } from './shared-portal-lib.component';

describe('SharedPortalLibComponent', () => {
  let component: SharedPortalLibComponent;
  let fixture: ComponentFixture<SharedPortalLibComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedPortalLibComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SharedPortalLibComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
