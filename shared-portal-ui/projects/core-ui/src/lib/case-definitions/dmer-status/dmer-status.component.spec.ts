import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DmerStatusComponent } from './dmer-status.component';

describe('DmerStatusComponent', () => {
  let component: DmerStatusComponent;
  let fixture: ComponentFixture<DmerStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DmerStatusComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DmerStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
