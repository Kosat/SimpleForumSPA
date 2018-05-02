import { Routes, RouterModule } from '@angular/router';
import { AuthService } from './shared/services/auth.service';
import { ForumComponent } from './forum/forum.component';
import { UsersListComponent } from './users-list/users-list.component';
import { ErrorPageComponent } from './shared/error-page/error-page.component';
import { ThreadCreateComponent } from './forum/thread-create/thread-create.component';
import { ThreadViewComponent } from './forum/thread-view/thread-view.component';

const appRoutes: Routes = [
  { path: '', redirectTo: '/users', pathMatch: 'full' },
  { path: 'users', component: UsersListComponent },
  { path: 'forum', component: ForumComponent },
  { path: 'forum/thread-create/:threadId', component: ThreadCreateComponent },
  { path: 'forum/thread-view/:threadId', component: ThreadViewComponent },
  { path: 'error', component: ErrorPageComponent },
  { path: '**', redirectTo: '/error?message=Invalid route' }

];
export const authProviders = [
    AuthService
];

export const routing = RouterModule.forRoot(appRoutes, { useHash: true });
