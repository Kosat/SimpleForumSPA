import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import { HttpHeaders } from '@angular/common/http';
import { Logger } from 'angular2-logger/core';
import { UserManager, User } from 'oidc-client';
import { ConfigService } from './config.service';


@Injectable()
export class AuthService {
  mgr: UserManager;
  userLoadededEvent$: EventEmitter<User> = new EventEmitter<User>();
  currentUser: User;
  loggedIn = false;

  constructor(
    private logger: Logger,
    private config: ConfigService) {

      const settings: any = {
      authority: `${config.getAuthApiUrl()}/`,
      client_id: 'js',
      redirect_uri: `${config.getSpaUrl()}/auth.html`,
      post_logout_redirect_uri: `${config.getSpaUrl()}/`,
      response_type: 'id_token token',
      scope: 'openid profile api.forum',
      silent_redirect_uri: `${config.getSpaUrl()}/silent-renew.html`,
      automaticSilentRenew: true,
      accessTokenExpiringNotificationTime: 4,
      filterProtocolClaims: true,
      loadUserInfo: true
    };

    this.mgr = new UserManager(settings);

    this.mgr.getUser()
      .then((user) => {
        if (user) {
          this.loggedIn = true;
          this.currentUser = user;
          this.userLoadededEvent$.emit(user);
        }
        else {
          this.loggedIn = false;
        }
      })
      .catch((err) => {
        this.loggedIn = false;
      });

    this.mgr.events.addUserLoaded((user) => {
      this.currentUser = user;
      this.loggedIn = !(user === undefined);
      this.logger.debug('authService addUserLoaded', user);
    });

    this.mgr.events.addUserUnloaded((e) => {
      this.logger.debug('user unloaded');
      this.loggedIn = false;
    });
  }

  get IsAuthorized(): boolean {
    return this.loggedIn;
  }

  get UserData(): any {
    return this.currentUser.profile;
  }

  public GetToken(): string {
    if (!this.loggedIn) {
      return '';
    }
    return this.currentUser.access_token;
  }

  public Login() {
    this.mgr.signinRedirect().then(() => {
      this.logger.info('SigninRedirect done');
    }).catch((err) => {
      this.logger.error(err);
    });
  }

  public Logoff() {
    this.mgr.getUser().then(user => {
      return this.mgr.signoutRedirect({ id_token_hint: user.id_token }).then(resp => {
        this.logger.info('Signed out', resp);
        this.userLoadededEvent$.emit(null);
      }).catch((err) => {
        this.logger.error(err);
      });
    });
  }

  public isLoginAsAdmin = (): boolean => this.isInRole('admin');
  public isLoginAsModerator = (): boolean => this.isInRole('moderator');
  public isLoginAsUser = (): boolean => this.isInRole('user');
  public isLoginAsSuperUser =
    (): boolean => this.isLoginAsModerator() || this.isLoginAsAdmin()

  private isInRole(roleName: string) {
    if (!this.loggedIn
      || !this.currentUser
      || !this.currentUser.profile
      || !this.currentUser.profile.role) {
      return false;
    }

    return this.currentUser.profile.role.indexOf(roleName) > -1;
  }


}
