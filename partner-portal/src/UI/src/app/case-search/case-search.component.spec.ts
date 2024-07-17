import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseSearchComponent } from './case-search.component';

describe('CaseSearchComponent', () => {
  let component: CaseSearchComponent;
  let fixture: ComponentFixture<CaseSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CaseSearchComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CaseSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
