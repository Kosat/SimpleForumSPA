import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/operator/map';
import { Component, OnInit, OnChanges, Output, Input, EventEmitter } from '@angular/core';
import { AuthService } from '../shared/services/auth.service';
import { Logger } from 'angular2-logger/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {

  private isUserDropdownOpened = false;
  private authenticated = false;
  private subscription: Subscription;
  private userName = '';
  private role = '';

  constructor(private logger: Logger, private authService: AuthService) {
    this.logger.debug('NavMenuComponent created.');
  }

  ngOnInit(): void {
    this.subscription = this.authService.userLoadededEvent$.subscribe(user => {
      this.logger.debug('Login user changed.');
      this.authenticated = this.authService.IsAuthorized;
      this.userName = this.authService.UserData.email;
    });

    this.logger.debug(`Identity component, checking authorized ${this.authService.IsAuthorized}`);
    this.authenticated = this.authService.IsAuthorized;

    if (this.authenticated) {
      if (this.authService.UserData) {
        this.userName = this.authService.UserData.email;
        const userRoles = this.authService.UserData.role;
        if (this.authService.isLoginAsAdmin()) {
          this.role = 'admin';
        }
        else if (this.authService.isLoginAsModerator()) {
          this.role = 'moderator';
        }
        else {
          this.role = 'user';
        }
      }
    }
  }

  toggleDropdown(value: boolean): void {
    this.isUserDropdownOpened = value;
  }

  login(evt: any) {
    this.logger.debug('Loging clicked');
    this.authService.Login();
  }

  logout(evt: any) {
    this.logger.debug('Logout clicked');
    this.toggleDropdown(false);
    this.authService.Logoff();
  }
}
