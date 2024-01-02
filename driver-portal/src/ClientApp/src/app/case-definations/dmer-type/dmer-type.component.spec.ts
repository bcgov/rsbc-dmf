import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DmerTypeComponent } from './dmer-type.component';

describe('DmerTypeComponent', () => {
  let component: DmerTypeComponent;
  let fixture: ComponentFixture<DmerTypeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DmerTypeComponent]
    });
    fixture = TestBed.createComponent(DmerTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
