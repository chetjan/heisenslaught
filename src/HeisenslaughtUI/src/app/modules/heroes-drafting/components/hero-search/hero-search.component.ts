import { Component, Input, Output, ChangeDetectorRef, EventEmitter } from '@angular/core';

import { HeroesService, HeroData } from '../../../heroes-data-service/heroes-data-service.module';
import { IDraftState } from '../../../heroes-draft-service/heroes-draft-service.module';

export * from './hero-filter.pipe';

@Component({
  selector: 'hero-search',
  template: `
    <div class="list">
      <hero-icon 
        *ngFor="let hero of heroes | heroSearch:searchField.value; let i = index;" [hero]="hero" 
        (click)="selectedHero = hero"
        [ngClass]="{selected: selectedHero === hero}"
        [class.picked]="isHeroPicked(hero.id)"
      ></hero-icon>
    </div>
   
     <div class="search">
      <div class="filters">
      </div>
      <div class="txtsearch">
        <input #searchField placeholder="Search..." (keyup)="search()" (focus)="searchFocus=true" (blur)="searchFocus=false"/>
        <button [class.focused]="searchFocus" [disabled]="!searchField.value" 
          (click)="searchField.value = ''; searchField.focus()"></button>
      </div>
    </div>
  `,
  styleUrls: ['./hero-search.component.scss']
})
export class HeroSearchComponent {
  private heroes: HeroData[];
  private _selectedHero: HeroData;

  @Output()
  public selectedHeroChange: EventEmitter<HeroData> = new EventEmitter<HeroData>();

  @Input()
  public state: IDraftState;

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

  public isHeroPicked(heroId: string) {
    if (this.state && this.state.picks) {
      if (this.state.picks.lastIndexOf(heroId) !== -1) {
        return true;
      }
      if (heroId === 'cho' || heroId === 'gall') {
        return this.state.picks.lastIndexOf(heroId === 'cho' ? 'gall' : 'cho') !== -1;
      }

    }
    return false;
  }
}
