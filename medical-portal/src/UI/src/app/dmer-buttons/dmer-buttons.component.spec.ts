import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DmerButtonsComponent } from './dmer-buttons.component';

describe('DmerButtonsComponent', () => {
  let component: DmerButtonsComponent;
  let fixture: ComponentFixture<DmerButtonsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DmerButtonsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DmerButtonsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
