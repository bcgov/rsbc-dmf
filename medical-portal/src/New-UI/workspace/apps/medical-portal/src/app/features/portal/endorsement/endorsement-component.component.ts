/* eslint-disable @typescript-eslint/explicit-member-accessibility */

/* eslint-disable @typescript-eslint/no-inferrable-types */
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatTable, MatTableDataSource } from '@angular/material/table';

import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

import { AuthService } from '@app/features/auth/services/auth.service';

import { EndorsementLicenseComponent } from './License/endorsement-license/endorsement-license.component';
import {
  Contact,
  EndorsementList,
  EndorsementResource,
  Licenses,
} from './endorsement-resource.service';

@Component({
  selector: 'app-endorsement-component',
  templateUrl: './endorsement-component.component.html',
  styleUrls: ['./endorsement-component.component.scss'],
  providers: [DialogService],
})
export class EndorsementComponentComponent implements OnInit {
  public dataSource: MatTableDataSource<EndorsementList>;
  public endorsements: EndorsementList[] = [];
  public Licenses: Licenses[] = [];
  public contact: Contact;
  moaLicense: string;
  usersRoles: string;
  ref: DynamicDialogRef = new DynamicDialogRef();
  userLicenses: Array<string>;
  pageSize = 10;
  display: boolean = false;

  paginationConfig = {
    pageSize: this.pageSize,
    pageNumber: 1,
    totalItems: this.endorsements.length,
  };
  public displayedColumns: string[] = [
    'hpdid',
    'licences',
    'contactId',
    'firstName',
    'lastName',
    'email',
    'birthDate',
    'role',
  ];

  content: string;
  public constructor(
    private endorsement: EndorsementResource,
    public dialogService: DialogService,
    private authService: AuthService
  ) {
    this.dataSource = new MatTableDataSource();
    this.contact = new Contact();
    this.usersRoles = '';
    this.moaLicense = '';
    this.content = '';
    this.userLicenses = new Array<string>();
  }

  public ngOnInit(): void {
    this.getEndorsements();
    this.usersRoles = '';
  }

  userHasRole(hpdid: string, expectedRole: string): boolean {
    if (this.usersRoles === '') {
      return false;
    }
    const userRole = this.endorsements.filter((e) => e.hpdid == hpdid)[0]
      .contact.role;

    if (userRole === expectedRole) {
      return true;
    } else {
      return false;
    }
  }
  getUserLicense(hpdid: string): void {
    this.Licenses = this.endorsements.filter(
      (e) => e.hpdid === hpdid
    )[0].licences;
    if (this.Licenses === undefined || this.Licenses.length === 0) {
      this.display = true;
    } else {
      this.ref = this.dialogService.open(EndorsementLicenseComponent, {
        header: 'License Information',
        style: {
          'margin-left': 'auto',
          'margin-right': 'var(--spacing-xxl)',
          'background-color': 'green !important',
          color: 'var(--highlight-text-color)',
        },
        data: this.Licenses,
        closable: true,
        modal: true,
        dismissableMask: true,
        contentStyle: { overflow: 'auto', 'max-height': '200px' },
        baseZIndex: 10000,
      });
    }
  }

  private getEndorsements(): void {
    const hpdid = this.authService.getHpdid();
    this.endorsement
      .getEndorsements(hpdid)
      .subscribe((endorsementsList: EndorsementList[]) => {
        this.dataSource.data = endorsementsList.sort();
        this.endorsements = endorsementsList;
        endorsementsList.map((contacts) => {
          this.contact = contacts.contact;
        });
      });
  }
}
