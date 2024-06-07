import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalFooterComponent } from './medical-footer.component';

describe('MedicalFooterComponent', () => {
  let component: MedicalFooterComponent;
  let fixture: ComponentFixture<MedicalFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalFooterComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MedicalFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
