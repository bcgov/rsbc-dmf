import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeniedComponent } from './denied.component';

describe('DeniedComponent', () => {
  let component: DeniedComponent;
  let fixture: ComponentFixture<DeniedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeniedComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeniedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
