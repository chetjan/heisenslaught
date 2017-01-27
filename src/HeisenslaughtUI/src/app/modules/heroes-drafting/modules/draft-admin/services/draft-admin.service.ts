import { Injectable } from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { IPageResult } from '../../../../../common/http';

@Injectable()
export class DraftAdminService {

  constructor(private http: Http) { }

  public getActiveDrafts(query?: string, sort?: string, page?: number, pageSize?: number): Promise<IPageResult<any>> {
    let search: URLSearchParams = new URLSearchParams();
    search.set('q', query);
    search.set('s', sort);
    search.set('page', page ? page.toString() : undefined);
    search.set('pageSize', pageSize ? pageSize.toString() : undefined);
    return this.http.get('api/admin/draft/active', {
      search: search
    }).map(res => res.json()).toPromise();
  }

  public getDrafts(query?: string, sort?: string, page?: number, pageSize?: number): Promise<IPageResult<any>> {
    let search: URLSearchParams = new URLSearchParams();
    search.set('q', query);
    search.set('s', sort);
    search.set('page', page ? page.toString() : undefined);
    search.set('pageSize', pageSize ? pageSize.toString() : undefined);
    return this.http.get('api/admin/draft', {
      search: search
    }).map(res => res.json()).toPromise();
  }

  public getDraft(id: string): Promise<any> {
    return this.http.get('api/admin/draft/' + id).map(res => res.json()).toPromise();
  }

  public getDraftStats(): Promise<any> {
    return this.http.get('api/admin/draft/stats').map(res => res.json()).toPromise();
  }
}
