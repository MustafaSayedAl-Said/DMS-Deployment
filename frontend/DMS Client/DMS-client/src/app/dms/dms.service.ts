import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPaginationDirectories } from '../shared/Models/PaginationDirectories';
import { catchError, map, of, throwError } from 'rxjs';
import { DirectoryParams } from '../shared/Models/DirectoryParams';
import { DocumentParams } from '../shared/Models/DocumentParams';
import { IPaginationDocuments } from '../shared/Models/PaginationDocuments';
import { response } from 'express';

@Injectable({
  providedIn: 'root',
})
export class DmsService {
  baseUrl = 'http://130.61.58.153:8080/api/';

  constructor(private http: HttpClient) {}

  getDirectories(DirectoryParams: DirectoryParams) {
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
    params = params.append('pageNumber', DirectoryParams.pageNumber.toString());
    params = params.append('pageSize', DirectoryParams.pageSize.toString());
    return this.http
      .get<IPaginationDirectories>(this.baseUrl + 'directories/user', {
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

  getDocuments(DocumentParams: DocumentParams) {
    let params = new HttpParams();
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);
    if (DocumentParams.sort) {
      params = params.append('Sort', DocumentParams.sort);
    }
    if (DocumentParams.search) {
      params = params.append('Search', DocumentParams.search);
    }
    params = params.append('DirectoryId', DocumentParams.directoryId);
    params = params.append('pageNumber', DocumentParams.pageNumber.toString());
    params = params.append('pageSize', DocumentParams.pageSize.toString());
    return this.http
      .get<IPaginationDocuments>(this.baseUrl + 'documents', {
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

  updateDirectoryName(id: number, newName: string) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);
    headers = headers.set('Content-Type', 'application/json');
    const body = JSON.stringify(newName);
    console.log(body);

    return this.http
      .patch(this.baseUrl + 'directories/' + id, body, {
        headers,
        responseType: 'text' as 'json',
      })
      .pipe(
        map((response) => {
          console.log('Response: ', response);
          return true;
        }),
        catchError((err) => {
          console.error('Error deleting directory:', err);
          // If there's an error, return false
          return of(false);
        })
      );
  }

  deleteDirectory(id: number) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http
      .delete(this.baseUrl + 'directories/' + id, {
        headers,
        responseType: 'text' as 'json',
      })
      .pipe(
        map((response) => {
          console.log('Response: ', response);
          return true;
        }),
        catchError((err) => {
          console.error('Error deleting directory:', err);
          // If there's an error, return false
          return of(false);
        })
      );
  }

  addDirectory(name: string, workspaceId: number) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http
      .post(
        this.baseUrl + 'directories',
        { name: name, workspaceId: workspaceId },
        { headers, responseType: 'text' as 'json' }
      )
      .pipe(
        map((response) => {
          console.log('Response: ', response);
          return true;
        }),
        catchError((err) => {
          console.error('Error Adding directory: ', err);
          return of(false);
        })
      );
  }

  updateDocumentVisibility(id: number) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);
    const body = { isPublic: true };

    return this.http
      .patch(this.baseUrl + 'documents/' + id, body, {
        headers,
        responseType: 'text' as 'json',
      })
      .pipe(
        map((response) => {
          console.log('Response: ', response);
          return true;
        }),
        catchError((err) => {
          console.error('Error Updating Visibility: ', err);
          return of(false);
        })
      );
  }

  deleteDocument(id: number) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http
      .delete(this.baseUrl + 'documents/' + id, {
        headers,
        responseType: 'text' as 'json',
      })
      .pipe(
        map((response) => {
          console.log('Response: ', response);
          return true;
        }),
        catchError((err) => {
          console.error('Error deleting document', err);
          return of(false);
        })
      );
  }

  addDocument(file: File, directoryId: number) {
    const formData = new FormData();
    formData.append('DirectoryId', directoryId.toString());
    formData.append('DocumentContent', file, file.name);

    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http
      .post(this.baseUrl + 'documents', formData, {
        headers,
        responseType: 'text' as 'json',
      })
      .pipe(
        map((response) => {
          console.log('Response: ', response);
          return true;
        }),
        catchError((err) => {
          console.error('Error Adding document: ', err);
          return of(false);
        })
      );
  }

  downloadDocumentById(id: number, name: string) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http
      .get(this.baseUrl + 'documents/download/' + id, {
        headers: headers,
        responseType: 'blob', // Set response type to 'blob'
        observe: 'response',
      })
      .pipe(
        map((response: HttpResponse<Blob>) => {
          // Create a blob from the response
          const blob = new Blob([response.body], { type: response.body.type });

          // Create a link element, set href to the blob URL, and trigger the download
          const downloadLink = document.createElement('a');
          const url = window.URL.createObjectURL(blob);
          downloadLink.href = url;
          downloadLink.download = name;
          document.body.appendChild(downloadLink);
          downloadLink.click();
          document.body.removeChild(downloadLink);

          // Revoke the object URL after the download
          window.URL.revokeObjectURL(url);

          return true;
        })
      );
  }

  previewDocument(id: number) {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + token);

    return this.http
      .get(this.baseUrl + 'documents/preview/' + id, {
        headers: headers,
        responseType: 'blob',
        observe: 'response',
      })
      .pipe(
        map((response: HttpResponse<Blob>) => {
          const url = window.URL.createObjectURL(response.body);
          return url;
        })
      );
  }
}
