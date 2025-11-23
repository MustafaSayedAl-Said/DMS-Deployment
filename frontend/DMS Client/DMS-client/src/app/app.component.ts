import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AccountService } from './account/account.service';
import { IWorkspace } from './shared/Models/Workspaces';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  previousUrl: string | null = null;
  showHeader: boolean = false;
  title = 'DMS';
  constructor(private router: Router, private accountService: AccountService) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.showHeader =
          !event.url.includes('/account/login') &&
          !event.url.includes('/account/register');
      }
    });
  }

  loadCurrentUser() {
    this.showHeader = false;
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('token');
      if (token) {
        this.accountService.loadCurrentUser(token).subscribe({
          next: () => {
            console.log('load successfully');
            this.loadWorkspace();
            this.showHeader = true;
            console.log('Show header: ' + this.showHeader);
          },
          error: (err) => {
            console.log(err);
          },
        });
      }
    }
  }
  loadWorkspace() {
    this.accountService.getWorkspace().subscribe({
      next: (workspace: IWorkspace) => {
        this.accountService.setWorkspaceName(workspace.name);
        this.accountService.setWorkspaceId(workspace.id);
        console.log('Workspace name loaded: ', workspace.name);
        console.log('Workspace id', workspace.id);
      },
      error: (err) => {
        console.log('Error loading workspace name', err);
      },
    });
  }

  ngOnInit(): void {
    this.loadCurrentUser();
  }
}
