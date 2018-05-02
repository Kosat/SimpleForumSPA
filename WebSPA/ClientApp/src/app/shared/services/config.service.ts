import { Injectable } from '@angular/core';
import { IConfiguration } from '../models/config.model';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/map';
import { Logger } from 'angular2-logger/core';


@Injectable()
export class ConfigService {
    serverSettings: IConfiguration;
    private settingsLoadedSource = new Subject();
    settingsLoaded$ = this.settingsLoadedSource.asObservable();
    isReady = false;

    constructor(private logger: Logger) { }

    async loadFromServer() {
      const baseURI = document.baseURI.endsWith('/') ? document.baseURI : `${document.baseURI}/`;
      const url = `${baseURI}config`;
      try {
        this.logger.debug('Loading settings from server. ' + url);
        const response = await fetch(url);
        if (!response.ok) {
          throw new Error(response.statusText); // Halt the app
        }
        const serverSettingsJson = await response.json();
        this.logger.debug(serverSettingsJson);
        this.serverSettings = <IConfiguration> serverSettingsJson.Endpoints;
        sessionStorage.setItem('api_auth', this.serverSettings.Api_Auth);
        sessionStorage.setItem('spa', this.serverSettings.Spa);
        this.isReady = true;
        this.logger.info('App settings loaded from server.');
        this.settingsLoadedSource.next();
      } catch (err) {
        console.error(`ConfigService failed on loading from ${url}`, err);
      }
    }

    CheckReadyGuard() {
      if (!this.isReady) {
        throw new Error('Settings are not ready.');
      }
    }

    getAuthApiUrl() {
      this.CheckReadyGuard();
      return sessionStorage.getItem('api_auth');
    }

    getSpaUrl() {
      this.CheckReadyGuard();
      return sessionStorage.getItem('spa');
    }
}
