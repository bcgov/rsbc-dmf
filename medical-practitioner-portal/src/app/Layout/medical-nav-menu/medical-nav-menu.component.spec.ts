import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalNavMenuComponent } from './medical-nav-menu.component';

describe('MedicalNavMenuComponent', () => {
  let component: MedicalNavMenuComponent;
  let fixture: ComponentFixture<MedicalNavMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalNavMenuComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MedicalNavMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
