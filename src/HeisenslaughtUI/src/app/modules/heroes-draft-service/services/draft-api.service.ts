import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class DraftService {

    constructor(
        private http: Http
    ) { }

    public getMyRecentlyJoinedDrafts(): Observable<any[]> {
        return this.http.get('api/draft/recent/joined').map(res => res.json());
    }

    public getMyRecentlyCreatedDrafts(): Observable<any[]> {
        return this.http.get('api/draft/recent/created').map(res => res.json());
    }

}

