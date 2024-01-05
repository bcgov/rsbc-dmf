import { Component, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';

@Component({
  selector: 'app-case',
  templateUrl: './case.component.html',
  styleUrls: ['./case.component.scss'],
})
export class CaseComponent {
  isExpanded: Record<string, boolean> = {
    '1': false,
  };
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  toggleisExpandable(id: string) {
    this.isExpanded[id] = !this.isExpanded[id];
  }
}
