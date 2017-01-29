import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { HeroData } from '../../../services/heroes.service';


@Injectable()
export class HeroesDataImporterService {
  private static _heroImportProgress: any;
  private static _isImportingHeroImages = false;

  constructor(private http: Http) {

  }

  public get isImportingHeroImages(): boolean {
    return HeroesDataImporterService._isImportingHeroImages;
  }

  public get heroImportProgress(): any {
    return HeroesDataImporterService._heroImportProgress;
  }

  public async importHeroes(heroes: HeroData[]): Promise<void> {
    HeroesDataImporterService._isImportingHeroImages = true;
    HeroesDataImporterService._heroImportProgress = true;
    for (let i = 0; i < heroes.length; i++) {
      let hero = heroes[i];
      HeroesDataImporterService._heroImportProgress = {
        name: hero.name,
        num: i + 1,
        total: heroes.length
      };
      await this.http.get('api/admin/herodata/heroes/' + hero.id + '/image/import?url=' + 
        encodeURIComponent(hero.iconSmall || hero['url'])).toPromise();
    }
    HeroesDataImporterService._heroImportProgress = null;
    HeroesDataImporterService._isImportingHeroImages = false;
  }

}
