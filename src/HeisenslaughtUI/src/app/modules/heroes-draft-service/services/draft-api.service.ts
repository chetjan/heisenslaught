import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

@Injectable()
export class DraftService {

    constructor(
        private http: Http
    ) { }

    public getMyRecentlyJoinedDrafts(): Promise<any[]> {
        return this.http.get('api/draft/recent/joined').map(res => res.json()).toPromise();
    }

    public getMyRecentlyCreatedDrafts(): Promise<any[]> {
        return this.http.get('api/draft/recent/created').map(res => res.json()).toPromise();
    }

}

