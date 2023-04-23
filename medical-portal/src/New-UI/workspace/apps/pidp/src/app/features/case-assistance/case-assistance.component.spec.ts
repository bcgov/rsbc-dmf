import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseAssistanceComponent } from './case-assistance.component';

describe('CaseAssistanceComponent', () => {
  let component: CaseAssistanceComponent;
  let fixture: ComponentFixture<CaseAssistanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CaseAssistanceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CaseAssistanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
