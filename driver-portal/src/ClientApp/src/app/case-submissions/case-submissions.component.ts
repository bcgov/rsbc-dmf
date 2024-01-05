import { ViewportScroller } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';

@Component({
  selector: 'app-case-submissions',
  templateUrl: './case-submissions.component.html',
  styleUrls: ['./case-submissions.component.css'],
})
export class CaseSubmissionsComponent {
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  constructor(private viewportScroller: ViewportScroller) {}

  public onClick(elementId: string): void {
    this.viewportScroller.scrollToAnchor(elementId);
  }
}
