import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-letters-to-driver',
  templateUrl: './letters-to-driver.component.html',
  styleUrls: ['./letters-to-driver.component.css'],
})
export class LettersToDriverComponent {
  pageSize = 10;

  filteredDocuments?: Document[] | null = [];

  private _letterDocuments?: Document[] | null = [];
  get letterDocuments() {
    return this._letterDocuments;
  }

  @Input()
  set letterDocuments(value) {
    this._letterDocuments = value;
    this.filteredDocuments = this._letterDocuments?.slice(
      0,
      this.pageSize
    );
  }

  @ViewChild(MatAccordion) accordion!: MatAccordion;

  viewMore() {
    const pageSize =
      (this.filteredDocuments?.length ?? 0) + this.pageSize;

    this.filteredDocuments = this._letterDocuments?.slice(0, pageSize);
    console.log(pageSize);
  }
}
