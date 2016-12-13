import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { HeroData } from './hero';

export { HeroData } from './hero';

@Injectable()
export class HeroesService {

  private heroData: Observable<HeroData[]>;
  constructor(
    private http: Http
  ) {


  }

  public getHeroes(): Observable<HeroData[]> {
    if (!this.heroData) {
      this.heroData = this.http.get('app/data/heroes.json')
        .map((res: Response) => res.json())
        .catch((error: any) => Observable.throw(error.json().error || 'Server error'));
    }
    return this.heroData;
  }

}
