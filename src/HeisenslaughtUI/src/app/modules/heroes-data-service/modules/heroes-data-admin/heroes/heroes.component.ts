import { Component, OnInit } from '@angular/core';
import { HeroesDataAdminService } from '../services/heroes-data.service';
import { HeroesDataImporterService } from '../services/heroes-data-importer.service';
@Component({
  selector: 'app-heroes',
  templateUrl: './heroes.component.html',
  styleUrls: ['./heroes.component.scss']
})
export class HeroesComponent implements OnInit {
  public heroes: any[];
  public importReport: any[];

  constructor(
    private heroesDataService: HeroesDataAdminService,
    private heroesDataImporterService: HeroesDataImporterService
  ) { }

  async ngOnInit() {
    [this.heroes, this.importReport] =
      await Promise.all([this.heroesDataService.getHeroes(), this.heroesDataService.getHeroImageImportReport()]);

  }

  public async import(heroesToImport) {
    await this.heroesDataImporterService.importHeroes(heroesToImport);
    this.importReport = await this.heroesDataService.getHeroImageImportReport();
  }

  public get isImporting(): boolean {
    return this.heroesDataImporterService.isImportingHeroImages;
  }

   public get importProgress(): any {
    return this.heroesDataImporterService.heroImportProgress;
  }
}
