import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, of, ReplaySubject } from 'rxjs';
import { IUser } from '../shared/Models/user';
import { Router } from '@angular/router';
import { IWorkspace } from '../shared/Models/Workspaces';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  _baseURL = 'http://localhost:8080/api/';
  private currentUser = new BehaviorSubject<IUser>(null);
  currentUser$ = this.currentUser.asObservable();
  private workspaceIdSubject = new BehaviorSubject<number>(null);
  private workspaceNameSubject = new BehaviorSubject<string>('My Workspace');
  workspaceName$ = this.workspaceNameSubject.asObservable();
  private workspaceId$ = this.workspaceIdSubject.asObservable();

  setWorkspaceName(name: string) {
    this.workspaceNameSubject.next(name);
  }
  setWorkspaceId(id: number) {
    this.workspaceIdSubject.next(id);
  }

  getWorkspaceNameFromSubject() {
    return this.workspaceName$;
  }
  getWorkspaceIdFromSubject() {
    return this.workspaceId$;
  }

  constructor(private http: HttpClient, private router: Router) {}

  getCurrentUserValue() {
    return this.currentUser.value;
  }
  loadCurrentUser(token: string) {
    if (token === null) {
      this.currentUser.next(null);

      return of(null);
    }
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http.get(this._baseURL + 'users/current', { headers }).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUser.next(user);
        }
      })
    );
  }

  login(value: any) {
    return this.http.post(this._baseURL + 'users/login', value).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUser.next(user);
          this.getWorkspace().subscribe({
            next: (workspace: IWorkspace) => {
              this.setWorkspaceName(workspace.name);
              this.setWorkspaceId(workspace.id);
            },
          });
        }
      })
    );
  }

  register(value: any) {
    console.log(value);
    return this.http.post(this._baseURL + 'users/register', value).pipe(
      map((user: IUser) => {
        if (user) {
          this.router.navigateByUrl('/');
          localStorage.setItem('token', user.token);
          this.currentUser.next(user);
          this.getWorkspace().subscribe({
            next: (workspace: IWorkspace) => {
              this.setWorkspaceName(workspace.name);
              this.setWorkspaceId(workspace.id);
            },
          });
        }
      })
    );
  }

  logout() {
    this.router.navigateByUrl('/');
    localStorage.removeItem('token');
    this.currentUser.next(null);
    this.workspaceNameSubject.next(null);
    this.workspaceIdSubject.next(null);
  }

  checkEmailExist(email: string) {
    return this.http.get(this._baseURL + 'users/check?email=' + email);
  }

  getWorkspace() {
    let headers = new HttpHeaders();
    const token = localStorage.getItem('token');
    if (token === null) {
      this.currentUser.next(null);
      return of(null);
    }
    headers = headers.set('Authorization', 'Bearer ' + token);
    return this.http
      .get<IWorkspace>(this._baseURL + 'users/workspace', { headers })
      .pipe(
        map((response) => {
          return response;
        })
      );
  }
}
