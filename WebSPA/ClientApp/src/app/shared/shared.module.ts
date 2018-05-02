// Modules:
import { NgModule, ModuleWithProviders, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule, JsonpModule } from '@angular/http';

// Services:
import { AuthService } from './services/auth.service';

// Components:
import { ErrorPageComponent } from './error-page/error-page.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppHttpInterceptor } from './services/app.http.interceptor';
import { ConfigService } from './services/config.service';

// Pipes:

export function ConfigLoader(configService: ConfigService) {
  return () => configService.loadFromServer();
}

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    HttpModule,
    JsonpModule
  ],
  declarations: [
    ErrorPageComponent,
  ],
  exports: [
    // Modules
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
  ]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        ConfigService,
        {
          provide: APP_INITIALIZER,
          useFactory: ConfigLoader,
          deps: [ConfigService],
          multi: true
        },
        AuthService,
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AppHttpInterceptor,
          multi: true
        },
      ]
    };
  }
}
