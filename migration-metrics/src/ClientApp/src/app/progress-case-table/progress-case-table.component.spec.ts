import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressCaseTableComponent } from './progress-case-table.component';

describe('ProgressCaseTableComponent', () => {
  let component: ProgressCaseTableComponent;
  let fixture: ComponentFixture<ProgressCaseTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressCaseTableComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ProgressCaseTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
