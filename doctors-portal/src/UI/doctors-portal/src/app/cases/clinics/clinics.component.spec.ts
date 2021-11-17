import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ConfigurationService } from 'src/app/shared/services/configuration.service';
import { ConfigurationStubService } from 'src/app/shared/stubs/configuration.service.stub';

import { ClinicsComponent } from './clinics.component';

describe('ClinicsComponent', () => {
  let component: ClinicsComponent;
  let fixture: ComponentFixture<ClinicsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ClinicsComponent ],
      imports: [
        RouterTestingModule,
        HttpClientTestingModule
      ],
      providers: [
        { provide: ConfigurationService, useClass: ConfigurationStubService }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ClinicsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
