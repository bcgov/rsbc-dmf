import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';

import { VersionInfoComponent } from './version-info.component';

@NgModule({
  declarations: [VersionInfoComponent],

  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class VersionInfoModule {}
