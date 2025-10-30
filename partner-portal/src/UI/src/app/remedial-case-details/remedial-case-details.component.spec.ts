import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RemedialCaseDetailsComponent } from './remedial-case-details.component';

describe('RemedialCaseDetailsComponent', () => {
  let component: RemedialCaseDetailsComponent;
  let fixture: ComponentFixture<RemedialCaseDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RemedialCaseDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RemedialCaseDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
