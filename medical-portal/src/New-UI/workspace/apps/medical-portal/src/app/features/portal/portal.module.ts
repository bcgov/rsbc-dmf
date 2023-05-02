import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSortModule } from '@angular/material/sort';

import { PermissionsModule } from '@app/modules/permissions/permissions.module';

import { SharedModule } from '@shared/shared.module';

import { PortalRoutingModule } from './portal-routing.module';
import { PortalPage } from './portal.page';

@NgModule({
  declarations: [PortalPage],
  imports: [
    PortalRoutingModule,
    SharedModule,
    PermissionsModule,
    FormsModule,
    ReactiveFormsModule,
    MatSortModule,
  ],
  schemas: [
    // This causes the compiler to allow the non-angular swiper html tags.
    // Without this schema, compiling will fail on the swiper tags.
    CUSTOM_ELEMENTS_SCHEMA,
  ],
})
export class PortalModule {}
