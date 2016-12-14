import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DraftService, IDraftConfig, IDraftState, DraftPhase } from '../services/draft.service';
import { HeroesService, HeroData, IMapData } from '../services/heroes.service';

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
  private maps: IMapData[];

  public team1Status: string;
  public team2Status: string;

  private draftToken: string;
  private teamToken: string;

  private team: number;

  constructor(
    private draftService: DraftService,
    private heroesService: HeroesService,
    private route: ActivatedRoute,
    private changeRef: ChangeDetectorRef
  ) {

    this.heroesService.getHeroes().subscribe((heroes) => {
      this.heroes = heroes;
    });
    this.heroesService.getMaps().subscribe((maps) => {
      this.maps = maps;
    });

    this.draftToken = this.route.snapshot.params['id'];
    this.teamToken = this.route.snapshot.params['team'];

    this.draftService.connectToDraft(this.draftToken, this.teamToken).then((config) => {
      this.draftConfig = config;
      this.team = config.team;
      this.updateState(config.state);
      this.draftService.getDraftConfig(this.draftToken).subscribe((cfg) => {
        this.draftConfig = cfg;
        this.updateState(cfg.state);
      });

    }, (err) => {
      console.log('connect error', err);
    });
  }

  private updateState(draftState: IDraftState) {
    this.draftState = draftState;
    console.log('update state', draftState);
    if (draftState.phase === DraftPhase.WAITING) {
      console.log('waiting update state......', draftState);
      if (draftState.team1Ready) {
        this.team1Status = 'Ready';
      } else {
        this.team1Status = null;
      }

      if (draftState.team2Ready) {
        this.team2Status = 'Ready';
      } else {
        this.team2Status = null;
      }
    } else if (draftState.phase !== DraftPhase.FINISHED) {
      this.team1Status = draftState.team1BonusTime.toString();
      this.team2Status = draftState.team2BonusTime.toString();
    }
    this.changeRef.detectChanges();
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


  public get mapName(): string {
    if (this.maps && this.draftConfig) {
      let map = this.maps.find((value) => {
        return value.id === this.draftConfig.map;
      });
      if (map) {
        return map.name;
      }
    }
    return '';
  }

  ngOnInit() {

  }

  public setReady() {
    this.draftService.setReady(this.draftToken, this.teamToken);
  }

  public get showReady(): boolean {
    if (!this.teamToken) {
      return false;
    }
    if (!this.draftState || !this.draftConfig) {
      return false;
    }

    if (this.draftState.team1Ready && this.team === 1) {
      return false;
    }

    if (this.draftState.team2Ready && this.team === 2) {
      return false;
    }

    return true;
  }

}
