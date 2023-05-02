import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';

import { SharedModule } from '@app/shared/shared.module';

import { EndorsementComponentComponent } from './endorsement-component.component';
import { EndorsementRoutingModuleModule } from './endorsement-routing-module.module';

@NgModule({
  declarations: [EndorsementComponentComponent],
  imports: [
    CommonModule,
    MatTableModule,
    SharedModule,
    CommonModule,
    FormsModule,
    DynamicDialogModule,
    EndorsementRoutingModuleModule,
    TableModule,
    ButtonModule,
    DropdownModule,
    InputTextModule,
    DialogModule,
    PaginatorModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class EndorsementModuleModule {}
