import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, isDevMode } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { Logger } from 'angular2-logger/core';
import { environment } from '../environments/environment';

import { SharedModule } from './shared/shared.module';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ForumComponent } from './forum/forum.component';
import { UsersListComponent } from './users-list/users-list.component';
import { GravatarComponent } from './shared/gravatar/gravatar.component';
import { ThreadSettingsComponent } from './forum/thread-settings/thread-settings.component';
import { ErrorPageComponent } from './shared/error-page/error-page.component';
import { ThreadCreateComponent } from './forum/thread-create/thread-create.component';
import { ThreadViewComponent } from './forum/thread-view/thread-view.component';

import { routing, authProviders } from './app.routing';
import { ForumService } from './forum/services/forum.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ForumComponent,
    UsersListComponent,
    GravatarComponent,
    ThreadSettingsComponent,
    ThreadCreateComponent,
    ThreadViewComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    routing,
    BsDropdownModule.forRoot(),
    SharedModule.forRoot(),
    BrowserAnimationsModule, // ToastrModule requires animations module
    ToastrModule.forRoot({
      timeOut: 5000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true}), // ToastrModule
  ],
  providers: [
    Logger,
    ForumService],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private logger: Logger) {
    if (isDevMode()) {
      window.console.info('To see debug logs enter: \'logger.level = logger.Level.DEBUG;\' in your browser console');
    }
    this.logger.level = environment.logger.level;
  }
 }
