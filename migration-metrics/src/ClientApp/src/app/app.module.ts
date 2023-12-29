import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { ProgressComponent } from './progress/progress.component';
import { ProgressChartComponent } from './progress-chart/progress-chart.component';
import { ProgressTableComponent } from './progress-table/progress-table.component';
import { ProgressCaseTableComponent } from './progress-case-table/progress-case-table.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    ProgressComponent,
    ProgressChartComponent,
    ProgressTableComponent,
    ProgressCaseTableComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'progress', component: ProgressComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ])

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
