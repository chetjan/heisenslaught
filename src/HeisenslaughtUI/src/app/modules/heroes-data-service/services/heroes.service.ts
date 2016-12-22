import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { HeroData } from './types/hero';
import { IMapData } from './types/map';

export { HeroData } from './types/hero';
export { IMapData } from './types/map';


@Injectable()
export class HeroesService {
  private heroData: Observable<HeroData[]>;
  private mapData: Observable<IMapData[]>;


  constructor(private http: Http) { }

  public getHeroes(): Observable<HeroData[]> {
    if (!this.heroData) {
      this.heroData = this.http.get('data/heroes.json')
        .map((res: Response) => res.json())
        .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
    return this.heroData;
  }

  public getMaps(): Observable<IMapData[]> {
    if (!this.mapData) {
      this.mapData = this.http.get('data/maps.json')
        .map((res: Response) => res.json())
        .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
    return this.mapData;
  }
}
