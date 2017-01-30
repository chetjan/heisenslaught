import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { RestService } from '../../../common/http/rest/rest.service';
import { HeroData } from './types/hero';
import { IMapData } from './types/map';

export { HeroData } from './types/hero';
export { IMapData } from './types/map';


@Injectable()
export class HeroesService extends RestService {
  private heroData: Promise<HeroData[]>;
  private mapData: Promise<IMapData[]>;

  private heroImages: Promise<{ [id: string]: string }>;
  private mapImages: Promise<{ [id: string]: string }>;

  constructor(http: Http) {
    super(http);
  }

  protected get baseUrl(): string {
    return 'api/herodata';
  }

  public getHeroes(): Promise<HeroData[]> {
    if (!this.heroData) {
      this.heroData = this.http.get('data/heroes.json')
        .map((res: Response) => res.json()).toPromise();
    }
    return this.heroData;
  }

  public getMaps(): Promise<IMapData[]> {
    if (!this.mapData) {
      this.mapData = this.http.get('data/maps.json')
        .map((res: Response) => res.json()).toPromise();
    }
    return this.mapData;
  }

  public getHeroImages(): Promise<{ [id: string]: string }> {
    if (!this.heroImages) {
      this.heroImages = this.get('heroes/images');
    }
    return this.heroImages;
  }

  public getMapImages(): Promise<{ [id: string]: string }> {
    if (!this.mapImages) {
      this.mapImages = this.get('maps/images');
    }
    return this.mapImages;
  }
}
