import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-letters-to-driver',
  templateUrl: './letters-to-driver.component.html',
  styleUrls: ['./letters-to-driver.component.scss'],
})
export class LettersToDriverComponent implements OnInit {
  constructor(private caseManagementService: CaseManagementService) {}

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  isExpanded: Record<string, boolean> = {};
  pageSize = 10;

  filteredDocuments?: Document[] | null = [];

  private _letterDocuments?: Document[] | null = [];
  get letterDocuments() {
    return this._letterDocuments;
  }

  @Input()
  set letterDocuments(documents: Document[] | null | undefined) {
    this._letterDocuments = documents;
    this._letterDocuments?.forEach((doc) => {
      if (doc.documentId) this.isExpanded[doc.documentId] = false;
    });

    this.filteredDocuments = this._letterDocuments?.slice(0, this.pageSize);
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
  }
  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  viewMore() {
    const pageSize = (this.filteredDocuments?.length ?? 0) + this.pageSize;

    this.filteredDocuments = this._letterDocuments?.slice(0, pageSize);
  }

  downloadLetters(documentId: string | null | undefined) {
    if (!documentId) return;
    this.caseManagementService
      .getDownloadDocument({ documentId })
      .subscribe((res) => {
        this.downloadFile(res);
      });
  }

  downloadFile(data : any) {
    const blob = new Blob([data], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    //window.open('https://path/to/file.extenstion', '_blank');
    //window.open(url, '_blank');
    const link = document.createElement('a');
    link.href = url;
    link.download = 'LetterOut.pdf';
    link.click();
  }
}
