import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PhsaFormViewerComponent } from './phsa-form-viewer.component';

describe('PhsaFormViewerComponent', () => {
  let component: PhsaFormViewerComponent;
  let fixture: ComponentFixture<PhsaFormViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PhsaFormViewerComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PhsaFormViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
