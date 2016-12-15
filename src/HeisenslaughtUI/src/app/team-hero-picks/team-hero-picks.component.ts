import { Component, Input, HostBinding } from '@angular/core';

import { IDraftConfig, IDraftState, DraftPhase } from '../services/draft.service';

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
  public state: IDraftState = {
    phase: DraftPhase.PICKING,
    pickTime: 0,
    team1BonusTime: 0,
    team2BonusTime: 0,
    team1Ready: true,
    team2Ready: true,
    picks: ['a', 'b', 'c', 'd', 'e']
  };

  @Input()
  public config: IDraftConfig = {
    bankTime: true,
    bonusTime: 100,
    disabledHeroes: null,
    firstPick: 1,
    map: '',
    pickTime: 10,
    team1Name: '',
    team2Name: '',
    state: null
  };



  constructor() { }

  public get currentPick(): number {
    return this.state && this.state.phase === DraftPhase.PICKING ? this.state.picks.length : -1;
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

  public getSlot(index):number{
    return this.slots ? this.slots[index] : -1;
  }

}
