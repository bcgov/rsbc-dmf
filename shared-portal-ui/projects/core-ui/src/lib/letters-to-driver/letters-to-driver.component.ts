// IMPORTANT keep this file identical to partner-portal letters-to-driver.component
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle } from '@angular/material/expansion';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { NgFor, NgClass, NgIf, DatePipe } from '@angular/common';
import { CaseTypeComponent } from '../case-definitions/case-type/case-type.component';
import {LetterTopicComponent} from '../case-definitions/letter-topic/letter-topic.component'
import { PortalsEnum, SubmittalStatusEnum } from '../app.model';
import { Document } from '../api';
import { SharedQuickLinksComponent } from '../quick-links/quick-links.component';

@Component({
    selector: 'app-shared-letters-to-driver',
    templateUrl: './letters-to-driver.component.html',
    styleUrls: ['./letters-to-driver.component.scss'],
    standalone: true,
    imports: [
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
        SharedQuickLinksComponent
    ],
})
export class SharedLettersToDriverComponent implements OnInit {

  @Input() caseManagementService: any;
  @Input()  portal!: PortalsEnum;

  PortalsEnum = PortalsEnum;

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
    this._letterDocuments?.forEach((doc:any) => {
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
      .subscribe((res:any) => {
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
