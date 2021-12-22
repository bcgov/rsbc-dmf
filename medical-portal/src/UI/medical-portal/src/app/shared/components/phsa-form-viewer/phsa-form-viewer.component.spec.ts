import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SharedModule } from '../../shared.module';

import { PhsaFormViewerComponent } from './phsa-form-viewer.component';

describe('PhsaFormViewerComponent', () => {
  let component: PhsaFormViewerComponent;
  let fixture: ComponentFixture<PhsaFormViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PhsaFormViewerComponent],
      imports: [SharedModule]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PhsaFormViewerComponent);
    component = fixture.componentInstance;
    component.formServerOptions = {
      emrVendorId: 'test',
      fhirServerUrl: 'fhirurl',
      formServerUrl: 'formurl'
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
