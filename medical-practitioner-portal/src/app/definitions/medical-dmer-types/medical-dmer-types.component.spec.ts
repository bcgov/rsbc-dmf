import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalDmerTypesComponent } from './medical-dmer-types.component';

describe('MedicalDmerTypesComponent', () => {
  let component: MedicalDmerTypesComponent;
  let fixture: ComponentFixture<MedicalDmerTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalDmerTypesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MedicalDmerTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
