// IMPORTANT keep this file identical to partner-portal letters-to-driver.component
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { CaseTypeComponent, LetterTopicComponent } from '@shared/core-ui';
import { SubmittalStatusEnum } from '@app/app.model';


@Component({
    selector: 'app-letters-to-driver',
    templateUrl: './letters-to-driver.component.html',
    styleUrls: ['./letters-to-driver.component.scss'],
    standalone: true,
    imports: [
        QuickLinksComponent,
        NgFor,
        MatCard,
        NgClass,
        MatCardContent,
        MatIcon,
        MatButton,
        NgIf,
        MatAccordion,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        CaseTypeComponent,
        LetterTopicComponent,
        DatePipe,
    ],
})
export class LettersToDriverComponent implements OnInit {
  constructor(private caseManagementService: CaseManagementService) 
    {}

  @ViewChild(MatAccordion) accordion!: MatAccordion;
  isExpanded: Record<string, boolean> = {};
  pageSize = 10;
  @Input() isLoading = true;
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

  letterOutDocuments: Document[] = [];

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.getLetterOutDocument();  
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

  getLetterOutDocument() {
    this.caseManagementService.getAllDriverDocuments()
      .subscribe((letterDocuments: any) => {
        if (!letterDocuments) {
          return;
        }
        this._letterDocuments = letterDocuments;
        this.letterOutDocuments = [];
        letterDocuments.forEach((letter: any) => {
          if (
            [SubmittalStatusEnum.Issued, SubmittalStatusEnum.Sent].includes(
              letter.submittalStatus as SubmittalStatusEnum,
            )
          ) {
            this.letterOutDocuments.push(letter);
          }
        });

        this.filteredDocuments = this.letterOutDocuments.slice(
          0,
          this.pageSize,
        );
        this.isLoading = false;
      });
  }
}
