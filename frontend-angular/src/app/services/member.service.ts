import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { MemberModel } from '../models/memberMode';
import { MemberUpdateDto } from '../models/memberUpdateDto';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  paginatedResult: PaginatedResult<MemberModel[]> = new PaginatedResult<MemberModel[]>();

  constructor(
    @Inject('apiUrl') private apiUrl: string,
    private httpClient: HttpClient
  ) { }

  getMembers(page?: number, itemsPerPage?: number, minAge?: number, maxAge?: number, gender?: string, orderBy?: string) {
    let params = new HttpParams();

    if (page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    if (orderBy !== null && gender !== null && minAge !== null && maxAge !== null) {
      params = params.append('minAge', minAge);
      params = params.append('maxAge', maxAge);
      params = params.append('gender', gender);
      params = params.append('orderBy', orderBy);
    }



    return this.httpClient.get<MemberModel[]>(this.apiUrl + 'users', { observe: 'response', params }).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
      })
    );

  }

  getMember(userName: string): Observable<MemberModel> {
    return this.httpClient.get<MemberModel>(this.apiUrl + 'users/' + userName);
  }

  updateMember(member: MemberUpdateDto) {
    let api = this.apiUrl + 'users/update';
    return this.httpClient.post(api, member);
  }

  setMainPhoto(userName: string, photoId: number) {
    let api = this.apiUrl + 'users/set-main-photo?userName=' + userName + "&photoId=" + photoId;
    return this.httpClient.get(api);
  }

  deletePhoto(userName: string, photoId: Number) {
    let api = this.apiUrl + 'users/delete-photo?userName=' + userName + "&photoId=" + photoId;
    return this.httpClient.get(api);
  }

  addLike(username: string) {
    return this.httpClient.post(this.apiUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string,pageNumber:number,pageSize:number) {
    let params = new HttpParams();
    params=params.append('pageNumber',pageNumber.toString());
    params=params.append('pageSize',pageSize.toString());
    params=params.append('predicate',predicate);

    return this.httpClient.get<Partial<MemberModel[]>>(this.apiUrl + 'likes', { observe: 'response', params }).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
      })
    );
  }

}
