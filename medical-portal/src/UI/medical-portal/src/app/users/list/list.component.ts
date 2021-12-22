import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CaseManagementService, DMERCase } from 'src/app/shared/services/case-management/case-management.service';

@Component({
  selector: 'app-users-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  public dataSource: User[] = [];

  constructor() { }

  public ngOnInit(): void {
    this.dataSource = [
      { id: "1", fullName: "Dr. Rajan Mehra", clinicName: "Victoria Downtown Clinic", role: "Medical Practitioner" },
      { id: "2", fullName: "Dr. Shelby Drew", clinicName: "Victoria Downtown Clinic", role: "Specialist" },
      { id: "3", fullName: "Devi Iyer, NP", clinicName: "Victoria Downtown Clinic", role: "Nurse Practitioner" },
      { id: "4", fullName: "Dr. Tarik Haiga", clinicName: "Victoria Downtown Clinic", role: "Medical Pracitioner" },
    ];
  }

}

export interface User {
  id: string;
  fullName: string;
  role: string;
  clinicName: string;

}

