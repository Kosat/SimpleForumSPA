import { Component } from '@angular/core';
import { Logger } from 'angular2-logger/core';
import { ConfigService } from './shared/services/config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'Simple Forum';

  constructor(private logger: Logger, private configService: ConfigService) {
    logger.debug('Loading AppComponent...');
  }
}
