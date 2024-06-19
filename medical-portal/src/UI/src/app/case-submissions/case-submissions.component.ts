import { CUSTOM_ELEMENTS_SCHEMA, Component, Input } from '@angular/core';
import { QuickLinksComponent } from '../quick-links/quick-links.component';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatCard, MatCardContent } from '@angular/material/card';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import {
  MatAccordion,
  MatExpansionPanel,
  MatExpansionPanelHeader,
  MatExpansionPanelTitle,
} from '@angular/material/expansion';
import { CaseDocument } from '@app/shared/api/models';

@Component({
  selector: 'app-case-submissions',
  standalone: true,
  imports: [
    QuickLinksComponent,
    RouterLink,
    RouterLinkActive,
    NgFor,
    MatCard,
    NgClass,
    MatCardContent,
    MatIcon,
    NgIf,
    MatAccordion,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
  ],
  providers: [DatePipe],
  templateUrl: './case-submissions.component.html',
  styleUrl: './case-submissions.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CaseSubmissionsComponent {
  pageSize = 10;

  isLoading = true;

  filteredDocuments: CaseDocument[] = [];

  _documents: CaseDocument[] = [];

  @Input()
  set documents(docs: CaseDocument[]) {
    this._documents = docs;
    this.filteredDocuments = this._documents?.slice(0, this.pageSize);
  }

  get documents() {
    return this._documents;
  }

  isExpanded: Record<string, boolean> = {};

  constructor(private datePipe: DatePipe) {}

  //#TODO Move this to backend
  getFormattedDate(date: string | undefined | null) {
    if (!date) {
      return ' ';
    }
    return this.datePipe.transform(date, 'longDate');
  }

  toggleIsExpandable(id?: string | null) {
    if (id) this.isExpanded[id] = !this.isExpanded[id];
  }

  viewMore() {
    const pageSize = (this.filteredDocuments?.length ?? 0) + this.pageSize;
    this.filteredDocuments = this.documents?.slice(0, pageSize);
  }
}
