import { Component, OnInit } from '@angular/core';
import { DMERCase } from 'src/app/shared/services/case-management/case-management.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  public dataSource: DMERCase[] = [];
  
  constructor() { }

  ngOnInit(): void {
  }

}
