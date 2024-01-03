import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DecisionOutcomeComponent } from './decision-outcome.component';

describe('DecisionOutcomeComponent', () => {
  let component: DecisionOutcomeComponent;
  let fixture: ComponentFixture<DecisionOutcomeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DecisionOutcomeComponent]
    });
    fixture = TestBed.createComponent(DecisionOutcomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
