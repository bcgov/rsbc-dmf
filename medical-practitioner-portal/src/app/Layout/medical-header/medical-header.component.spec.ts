import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalHeaderComponent } from './medical-header.component';

describe('MedicalHeaderComponent', () => {
  let component: MedicalHeaderComponent;
  let fixture: ComponentFixture<MedicalHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalHeaderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MedicalHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
