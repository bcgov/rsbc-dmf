import { Routes } from '@angular/router';
import { RemedialComponent } from '@app/remedial/remedial.component';
import { SearchComponent } from '@app/search/search.component';

export const ROUTE_DENIED = 'denied';
export const routes: Routes = [
  { path: '', redirectTo: 'search', pathMatch: 'full' },

  {
    path: '',

    children: [
      { path: 'search', component: SearchComponent },
      { path: 'remedial', component: RemedialComponent },
    ],
  },
];
