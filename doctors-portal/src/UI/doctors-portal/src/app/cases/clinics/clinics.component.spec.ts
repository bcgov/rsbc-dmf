import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClinicsComponent } from './clinics.component';

describe('ClinicsComponent', () => {
  let component: ClinicsComponent;
  let fixture: ComponentFixture<ClinicsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ClinicsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ClinicsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
