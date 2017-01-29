/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { HeroesDataAdminService } from './heroes-data.service';

describe('HeroesDataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HeroesDataAdminService]
    });
  });

  it('should ...', inject([HeroesDataAdminService], (service: HeroesDataAdminService) => {
    expect(service).toBeTruthy();
  }));
});
