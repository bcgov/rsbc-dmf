import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {

  public notifications = Array<Notification>();
  constructor() { }

  ngOnInit(): void {
    this.notifications = [
      { level: 'Urgent', from: 'RoadSafetyBC', subject: 'Incorrect report submitted', caseId: '1234', date: 'October 5, 2021' },
      { level: 'Information', from: 'RoadSafetyBC', subject: 'Request for further information', caseId: '1234', date: 'October 12, 2021' },
    ];
  }

}

export class Notification {
  public from: string = '';
  public caseId: string= '';
  public subject: string= '';
  public date: string = '';
  public level: string = 'Urgent';
}
