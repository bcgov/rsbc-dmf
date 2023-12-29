import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressCaseChartComponent } from './progress-case-chart.component';

describe('ProgressCaseChartComponent', () => {
  let component: ProgressCaseChartComponent;
  let fixture: ComponentFixture<ProgressCaseChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressCaseChartComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ProgressCaseChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
