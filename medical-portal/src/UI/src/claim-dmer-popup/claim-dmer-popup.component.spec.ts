import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClaimDmerPopupComponent } from './claim-dmer-popup.component';

describe('ClaimDmerPopupComponent', () => {
  let component: ClaimDmerPopupComponent;
  let fixture: ComponentFixture<ClaimDmerPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClaimDmerPopupComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ClaimDmerPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
