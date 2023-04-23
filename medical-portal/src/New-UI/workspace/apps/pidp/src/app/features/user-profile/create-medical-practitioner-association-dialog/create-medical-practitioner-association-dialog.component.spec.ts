import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMedicalPractitionerAssociationDialogComponent } from './create-medical-practitioner-association-dialog.component';

describe('CreateMedicalPractitionerAssociationDialogComponent', () => {
  let component: CreateMedicalPractitionerAssociationDialogComponent;
  let fixture: ComponentFixture<CreateMedicalPractitionerAssociationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateMedicalPractitionerAssociationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMedicalPractitionerAssociationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
