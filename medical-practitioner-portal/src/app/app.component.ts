import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Layout/header/header.component';
import { FooterComponent } from './Layout/footer/footer.component';
import { NavMenuComponent } from './Layout/nav-menu/nav-menu.component';
import { CoreUiComponent } from '../../../shared-portal-ui/projects/core-ui/src/lib/core-ui.component';
import { CoreUiModule } from '../../../shared-portal-ui/projects/core-ui/src/lib/core-ui.module';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, NavMenuComponent ],
  //exports : [],
  //declarations :[CoreUiModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})


export class AppComponent {
  title = 'medical-practitioner-portal';
}
