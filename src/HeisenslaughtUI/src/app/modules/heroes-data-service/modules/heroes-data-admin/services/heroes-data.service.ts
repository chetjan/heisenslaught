import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { HeroesService } from '../../../services/heroes.service';

@Injectable()
export class HeroesDataAdminService extends HeroesService {

  constructor(http: Http) {
    super(http);
  }

  protected get baseUrl(): string {
    return 'api/admin/herodata';
  }
  public getHeroImageImportReport(): Promise<any> {
    return this.get('heroes/images/report');
  }

}
