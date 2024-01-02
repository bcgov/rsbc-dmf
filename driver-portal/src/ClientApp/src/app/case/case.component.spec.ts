import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseComponent } from './case.component';

describe('CaseComponent', () => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  let component: CaseComponent;
  let fixture: ComponentFixture<CaseComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CaseComponent],
    });
    fixture = TestBed.createComponent(CaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    //   expect(component).toBeTruthy();
  });
});
