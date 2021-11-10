import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfigurationService } from 'src/app/shared/services/configuration.service';
import { LoginService } from 'src/app/shared/services/login.service';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

  constructor(
    private configService: ConfigurationService,
    private route: ActivatedRoute,
    private loginService: LoginService
  ) { }

  public ngOnInit(): void {
    return;
  }

}
