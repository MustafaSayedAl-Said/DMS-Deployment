import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { DocumentParams } from "../shared/Models/DocumentParams";
import { IPaginationDocuments } from "../shared/Models/PaginationDocuments";
import { map } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class PublicService {
  baseUrl = 'http://130.61.58.153:8080/api/';

  constructor(private http: HttpClient) {}

  getPublicDocuments(DocumentParams: DocumentParams){
    let params = new HttpParams();
    if(DocumentParams.sort){
      //Sort
      params = params.append('Sort', DocumentParams.sort);
    }
    if(DocumentParams.search){
      params = params.append('Search', DocumentParams.search);
    }
    params = params.append('pageNumber', DocumentParams.pageNumber.toString());
    params = params.append('pageSize', DocumentParams.pageSize.toString());
    return this.http.get<IPaginationDocuments>(this.baseUrl + 'documents/public', {observe: 'response', params})
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

}
