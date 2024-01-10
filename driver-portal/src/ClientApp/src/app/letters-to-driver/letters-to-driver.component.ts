import { Component, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { CaseManagementService } from '../shared/services/case-management/case-management.service';
import { Document } from '../shared/api/models';

@Component({
  selector: 'app-letters-to-driver',
  templateUrl: './letters-to-driver.component.html',
  styleUrls: ['./letters-to-driver.component.css'],
})
export class LettersToDriverComponent implements OnInit {
  @ViewChild(MatAccordion) accordion!: MatAccordion;

  public letterDocuments: Document[] = [];
  constructor(private caseManagementService: CaseManagementService) {}

  ngOnInit(): void {
    this.getLetterDocuments('e27d7c69-3913-4116-a360-f5e990200173');
  }

  getLetterDocuments(driverId: string) {
    this.caseManagementService
      .getDriverDocuments({ driverId })
      .subscribe((letters: any) => {
        if (letters?.lettersToDriver && letters?.lettersToDriver?.length > 0) {
          this.letterDocuments = letters.lettersToDriver;
        }

        console.log(letters);
      });
  }
}
