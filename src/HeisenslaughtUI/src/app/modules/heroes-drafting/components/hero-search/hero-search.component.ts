import { Component, Input, Output, ChangeDetectorRef, EventEmitter, ViewChildren, QueryList } from '@angular/core';
import { MdButtonToggle } from '@angular/material/button-toggle';
import { HeroesService, HeroData } from '../../../heroes-data-service/heroes-data-service.module';
import { IDraftState } from '../../../heroes-draft-service/heroes-draft-service.module';

export * from './hero-filter.pipe';

@Component({
  selector: 'hero-search',
  templateUrl: './hero-search.component.html',
  styleUrls: ['./hero-search.component.scss']
})
export class HeroSearchComponent {
  private heroes: HeroData[];
  private _selectedHero: HeroData;

  @Output()
  public selectedHeroChange: EventEmitter<HeroData> = new EventEmitter<HeroData>();

  @Input()
  public state: IDraftState;

  @ViewChildren(MdButtonToggle)
  private filterGroup: QueryList<MdButtonToggle>;

  public roleFilters: any[] = [
    {
      label: 'Tank',
      values: ['tank']
    },
    {
      label: 'Bruiser',
      values: ['bruiser']
    },
    {
      label: 'Burst',
      values: ['burst']
    },
    {
      label: 'Sustain',
      values: ['sustain']
    },
    {
      label: 'Healer',
      values: ['healer']
    },
    {
      label: 'Support',
      values: ['support']
    },
    {
      label: 'Siege',
      values: ['siege']
    }

  ];

  public currentRoleFilter: string[];

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

  public onFilterChanged() {
    let filter = [];
    this.filterGroup.forEach((button, index) => {
      if (button.checked) {
        filter = filter.concat(button.value);
      }
    });
    this.currentRoleFilter = filter;
  }

}
