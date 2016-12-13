import { Component, OnInit } from '@angular/core';

import { DraftService, IDraftConfig, IDraftState, DraftPhase } from '../services/draft.service';
import { HeroesService, HeroData } from '../services/heroes.service';

@Component({
  selector: 'draft-screen',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent implements OnInit {

  public selectedHero: any;
  public draftConfig: IDraftConfig;
  public draftState: IDraftState;
  private heroes: HeroData[];

  public team1Status: string;
  public team2Status: string;


  constructor(
    private draftService: DraftService,
    private heroesService: HeroesService
  ) {

    this.heroesService.getHeroes().subscribe((heroes) => {
      this.heroes = heroes;
    });

    this.draftService.getDraftConfig('asdf').subscribe((draftConfig) => {
      this.draftConfig = draftConfig;
    });
    this.draftService.getDraftState('asdf').subscribe((draftState) => {
      this.draftState = draftState;

      if (draftState.phase === DraftPhase.WAITING) {
        if (draftState.team1Ready) {
          this.team1Status = 'Ready';
        }

        if (draftState.team2Ready) {
          this.team2Status = 'Ready';
        }
      } else if (draftState.phase !== DraftPhase.FINISHED) {
        this.team1Status = draftState.team1BonusTime.toString();
        this.team2Status = draftState.team2BonusTime.toString();
      }


    });
  }

  public getPick(team: number, pickId: number): HeroData {
    if (!this.heroes || !this.draftConfig || !this.draftState) {
      return null;
    }
    let picks = this.draftState.picks || [];
    let pickedHeroId: string;

    let pickSlots: number[];


    let fp = this.draftConfig.firstPick - 1;

    if ((team === fp)) {
      pickSlots = [2, 5, 6, 11, 12];
    } else {
      pickSlots = [3, 4, 9, 10, 13];
    }

    pickedHeroId = picks[pickSlots[pickId]];
    return this.getHeroById(pickedHeroId);
  }

  public getBan(team: number, pickId: number): HeroData {
    if (!this.heroes || !this.draftConfig || !this.draftState) {
      return null;
    }
    let picks = this.draftState.picks || [];
    let pickedHeroId: string;

    let banSlots: number[];


    let fp = this.draftConfig.firstPick - 1;

    if ((team === fp)) {
      banSlots = [0, 8];
    } else {
      banSlots = [1, 7];
    }

    pickedHeroId = picks[banSlots[pickId]];
    return this.getHeroById(pickedHeroId);
  }


  private getHeroById(heroId: string): HeroData {
    if (!this.heroes) {
      return null;
    }
    return this.heroes.find((value) => {
      return value.id === heroId;
    });
  }


  ngOnInit() {

  }

}
