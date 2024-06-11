import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  Input,
  CUSTOM_ELEMENTS_SCHEMA,
  Output,
  EventEmitter,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  imports: [RouterLink, MatIconModule, MatMenuModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class HeaderComponent {
  @Input() showProfile = false;

  showMobileMenu = false;

  @Input() profileName?: string;
  @Input() profileRole?: string;
  @Input() profileInitials?: string;
  @Output() logout = new EventEmitter();

  constructor() {}

  public logOut(): void {
    //this.loginService.logout();
    this.logout.emit();
  }
}
