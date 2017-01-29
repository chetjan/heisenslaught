import { Injectable, EventEmitter } from '@angular/core';
import { ImportWebWorker, _httpPost, _importImage } from './import-worker';
import { HeroData, IMapData } from '../../../services/heroes.service';
const httpPost = _httpPost;
const importImage = _importImage;

@Injectable()
export class HeroesDataImporterService {
  private static _heroImportWorker: ImportWebWorker;
  private static _heroImportProgress: any;


  constructor() {

  }

  public get isImportingHeroImages(): boolean {
    return HeroesDataImporterService._heroImportWorker && HeroesDataImporterService._heroImportWorker.working;
  }

  public get heroImportProgress(): any {
    return HeroesDataImporterService._heroImportProgress;
  }

  public importHeroes(heroes: HeroData[]): void {
    if (!HeroesDataImporterService._heroImportWorker) {
      HeroesDataImporterService._heroImportWorker = new ImportWebWorker(async (heroesToImport: HeroData[]) => {
        console.log('Importing', heroesToImport.length, 'Heroes');
        for (let i = 0; i < heroesToImport.length; i++) {
          let hero = heroesToImport[i];
          (<any>postMessage)({
            name: hero.name,
            num: i + 1,
            total: heroesToImport.length
          });
          await importImage(hero.id, 'heroes', hero.iconSmall);
        }
        (<any>postMessage)('done');
      });
      HeroesDataImporterService._heroImportWorker.data.subscribe((data) => {
        HeroesDataImporterService._heroImportProgress = data;
      });
    }
    HeroesDataImporterService._heroImportWorker.doWork(heroes);
  }

}
