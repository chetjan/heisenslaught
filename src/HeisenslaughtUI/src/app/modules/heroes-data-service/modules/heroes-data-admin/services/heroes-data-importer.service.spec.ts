/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { HeroesDataImporterService } from './heroes-data-importer.service';

describe('HeroesDataImporterService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HeroesDataImporterService]
    });
  });

  it('should ...', inject([HeroesDataImporterService], (service: HeroesDataImporterService) => {
    expect(service).toBeTruthy();
  }));
});
