import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import { ConfigurationService } from 'src/app/shared/services/configuration.service';
import { LoginService } from 'src/app/shared/services/login.service';
import { ConfigurationStubService } from 'src/app/shared/stubs/configuration.service.stub';
import { LoginStubService } from 'src/app/shared/stubs/login.service.stub';

import { HeaderComponent } from './header.component';

describe('HeaderComponent', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HeaderComponent],
      providers: [
        { provide: ConfigurationService, useClass: ConfigurationStubService },
        { provide: LoginService, useClass: LoginStubService }
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
