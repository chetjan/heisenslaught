import { Component, Input, Output, ChangeDetectorRef, EventEmitter } from '@angular/core';

import { HeroesService, HeroData } from '../../../heroes-data-service/heroes-data-service.module';

export * from './hero-filter.pipe';

@Component({
  selector: 'hero-search',
  template: `
    <div class="list">
      <hero-icon 
        *ngFor="let hero of heroes | heroSearch:searchField.value; let i = index;" [hero]="hero" 
        (click)="selectedHero = hero"
        [ngClass]="{selected: selectedHero === hero}"
      ></hero-icon>
    </div>
   
     <div class="search">
      <div class="filters">
      </div>
      <div class="txtsearch">
        <input #searchField (keyup)="search()"/>
      </div>
    </div>
  `,
  styleUrls: ['./hero-search.component.css']
})
export class HeroSearchComponent {
  private heroes: HeroData[];
  private _selectedHero: HeroData;

  @Output()
  public selectedHeroChange: EventEmitter<HeroData> = new EventEmitter<HeroData>();

  constructor(
    private heroesService: HeroesService,
    private ref: ChangeDetectorRef

  ) {
    heroesService.getHeroes().subscribe((data) => {
      this.heroes = data;
    });
  }

  @Input()
  public get selectedHero(): HeroData {
    return this._selectedHero;
  }

  public set selectedHero(value: HeroData) {
    if (this._selectedHero !== value) {
      this._selectedHero = value;
      this.selectedHeroChange.emit(this._selectedHero);
    }
  }

  public search() { }
}
