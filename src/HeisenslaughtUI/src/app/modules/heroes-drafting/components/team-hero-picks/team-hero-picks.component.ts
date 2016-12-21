import { Component, Input, HostBinding } from '@angular/core';

import { IDraftConfigDTO, IDraftState, DraftPhase } from '../../../heroes-draft-service/heroes-draft-service.module';
import { HeroesService, HeroData } from '../../../heroes-data-service/heroes-data-service.module';


@Component({
  selector: 'team-hero-picks',
  templateUrl: './team-hero-picks.component.html',
  styleUrls: ['./team-hero-picks.component.scss']
})
export class TeamHeroPicksComponent {


  @Input()
  @HostBinding('attr.team')
  public team: number;

  @Input()
  public slots: number[];

  @Input()
  public state: IDraftState;

  @Input()
  public config: IDraftConfigDTO;

  private heroes: HeroData[];


  constructor(private heroesService: HeroesService) {
    heroesService.getHeroes().subscribe((heroes) => {
      this.heroes = heroes;
    });

  }

  public get currentPick(): number {
    return this.state && this.state.phase !== DraftPhase.WAITING ? this.state.picks.length : -1;
  }

  public get currentPickIndex(): number {
    return this.slots.indexOf(this.currentPick);
  }

  public get nextPickIndex(): number {
    let currIdx = this.currentPickIndex;
    if (currIdx !== -1) {
      let nextIdx = currIdx + 1;
      if (this.slots[nextIdx] === this.currentPick + 1) {
        return nextIdx;
      }
    }
    return -1;
  }

  public get isTeamPicking(): boolean {
    return this.slots.indexOf(this.currentPick) !== -1;
  }

  public getSlot(index): number {
    return this.slots ? this.slots[index] : -1;
  }

  private getHeroById(heroId: string): HeroData {
    if (this.heroes) {
      return this.heroes.find((value) => {
        return value.id === heroId;
      });
    }
    return null;
  }
  getPick(index) {
    if (this.slots && this.heroes && this.state && this.state.picks) {
      let pickedHeroId = this.state.picks[this.slots[index]];
      if (pickedHeroId === 'failed_ban') {
        return {
          id: 'failed_ban',
          iconSmall: null,
          keywords: [],
          roles: [],
          name: 'Failed Ban'
        };
      }
      return this.getHeroById(pickedHeroId);
    }
    return null;
  }

}
