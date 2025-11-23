import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PublicComponent } from './public/public.component';
import { TestErrorComponent } from './core/test-error/test-error.component';
import { NotFoundComponent } from './core/not-found/not-found.component';
import { ServerErrorComponent } from './core/server-error/server-error.component';
import { AuthGuard } from './core/guards/auth.guard';
import { adminAuthGuard } from './core/guards/admin-auth.guard';
import { UsersComponent } from './dms/admin/users/users.component';
import { WorkspaceContentComponent } from './dms/admin/workspace-content/workspace-content.component';
import { DirectoryContentsComponent } from './dms/directory-contents/directory-contents.component';
import { ActionLogsComponent } from './dms/admin/action-logs/action-logs.component';

const routes: Routes = [
  {
    path: '',
    component: PublicComponent,
    data: { breadcrumb: null },
    pathMatch: 'full',
  },
  {
    path: 'not-found',
    component: NotFoundComponent,
    data: { breadcrumb: 'Not Found' },
  },
  {
    path: 'server-error',
    component: ServerErrorComponent,
    data: { breadcrumb: 'Server Error' },
  },
  {
    path: 'test-error',
    component: TestErrorComponent,
    data: { breadcrumb: 'Test Error' },
  },
  {
    path: 'dms',
    canActivate: [AuthGuard],
    loadChildren: () => import('./dms/dms.module').then((mo) => mo.DmsModule),
    data: { breadcrumb: 'Workspace' },
  },
  {
    path: 'users',
    loadChildren:() =>
      import('./dms/admin/admin.module').then((mo) => mo.AdminModule),
    canActivate: [adminAuthGuard],
    data: { breadcrumb: 'Users' },
  },
  {
    path: 'logs',
    canActivate: [adminAuthGuard],
    component: ActionLogsComponent,
    data: { breadcrumb: 'Action Logs' },
  },

  {
    path: 'account',
    loadChildren: () =>
      import('./account/account.module').then((mo) => mo.AccountModule),
    data: { breadcrumb: null },
  },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
