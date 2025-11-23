import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserParams } from '../../shared/Models/UserParams';
import { IPAginationUsers } from '../../shared/Models/PaginationUsers';
import { BehaviorSubject, catchError, map, of, throwError } from 'rxjs';
import { response } from 'express';
import { DirectoryParams } from '../../shared/Models/DirectoryParams';
import { IPaginationDirectories } from '../../shared/Models/PaginationDirectories';
import { IWorkspace } from '../../shared/Models/Workspaces';
import { ActionLog } from '../../shared/Models/ActionLog';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = 'http://130.61.58.153:8080/api/';
  constructor(private http: HttpClient) {}

  getUsersAndWorkspaces(UserParams: UserParams) {
    let params = new HttpParams();
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    if (UserParams.sort) {
      params = params.append('Sort', UserParams.sort);
    }
    if (UserParams.search) {
      params = params.append('Search', UserParams.search);
    }
    params = params.append('pageNumber', UserParams.pageNumber.toString());
    params = params.append('pageSize', UserParams.pageSize.toString());
    return this.http
      .get<IPAginationUsers>(this.baseUrl + 'users', {
        headers: headers,
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  getDirectoriesInWorkspace(DirectoryParams: DirectoryParams) {
    let params = new HttpParams();
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);
    if (DirectoryParams.sort) {
      //Sort=NameAsc
      params = params.append('Sort', DirectoryParams.sort);
    }
    if (DirectoryParams.search) {
      params = params.append('Search', DirectoryParams.search);
    }
    params = params.append('WorkspaceId', DirectoryParams.workspaceId);
    params = params.append('pageNumber', DirectoryParams.pageNumber.toString());
    params = params.append('pageSize', DirectoryParams.pageSize.toString());
    return this.http
      .get<IPaginationDirectories>(this.baseUrl + 'directories', {
        headers: headers,
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  getWorkspaceDetails(directoryId: number) {
    let headers = new HttpHeaders();
    const token = localStorage.getItem('token');
    headers = headers.set('Authorization', 'Bearer ' + token);
    return this.http
      .get<IWorkspace>(this.baseUrl + 'directories/workspace/' + directoryId, {
        headers: headers,
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  updateUserLock(id: number) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);
    const body = { isLocked: true };

    return this.http
      .patch(this.baseUrl + 'users/' + id, body, { headers })
      .pipe(
        map(() => {
          return true;
        }),
        catchError((err) => {
          console.error('Error Updating lock: ', err);
          return of(false);
        })
      );
  }

  getActionLogs() {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);
    return this.http
      .get<any[]>(this.baseUrl + 'actionlogs', {
        headers: headers,
        observe: 'response',
      })
      .pipe(
        map((response) => {
          return response.body;
        }),
        catchError((error) => {
          console.error('Error fetching action logs:', error);
          return throwError(() => new Error('Failed to fetch action logs.'));
        })
      );
  }
}
