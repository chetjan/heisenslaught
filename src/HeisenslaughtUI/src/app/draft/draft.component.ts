import { Component, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DraftService, IDraftConfig, IDraftState, DraftPhase } from '../services/draft.service';
import { HeroesService, HeroData, IMapData } from '../services/heroes.service';

@Component({
  selector: 'draft-screen',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent {

  private static firstSlots: number[] = [0, 2, 5, 6, 8, 11, 12];
  private static secondSlots: number[] = [1, 2, 3, 7, 9, 10, 13];
  private static firstPickSlots: number[] = [2, 5, 6, 11, 12];
  private static secondPickSlots: number[] = [3, 4, 9, 10, 13];
  private static firstBanSlots: number[] = [0, 8];
  private static secondBanSlots: number[] = [1, 7];


  private heroes: HeroData[];
  private maps: IMapData[];
  private draftToken: string;
  private teamToken: string;
  private team: number;
  private teamSlots: Array<number[]> = [];
  private teamPickSlots: Array<number[]> = [];
  private teamBanSlots: Array<number[]> = [];

  public selectedHero: any;
  public draftConfig: IDraftConfig;
  public draftState: IDraftState;
  public team1Status: string;
  public team2Status: string;


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
      this.configSlots();
      this.updateState(config.state);
      this.draftService.getDraftConfig(this.draftToken).subscribe((cfg) => {
        this.draftConfig = cfg;
        this.configSlots();
        this.updateState(cfg.state);
      });
      this.draftService.getDraftState(this.draftToken).subscribe((state) => {
        this.updateState(state);
      });
    }, (err) => {
      console.log('connect error', err);
    });
  }

  private configSlots(): void {
    if (this.draftConfig.firstPick === 1) {
      this.teamSlots[0] = DraftComponent.firstSlots;
      this.teamSlots[1] = DraftComponent.secondSlots;
      this.teamPickSlots[0] = DraftComponent.firstPickSlots;
      this.teamPickSlots[1] = DraftComponent.secondPickSlots;
      this.teamBanSlots[0] = DraftComponent.firstBanSlots;
      this.teamBanSlots[1] = DraftComponent.secondBanSlots;
    } else {
      this.teamSlots[0] = DraftComponent.secondSlots;
      this.teamSlots[1] = DraftComponent.firstSlots;
      this.teamPickSlots[0] = DraftComponent.secondPickSlots;
      this.teamPickSlots[1] = DraftComponent.firstPickSlots;
      this.teamBanSlots[0] = DraftComponent.secondBanSlots;
      this.teamBanSlots[1] = DraftComponent.firstBanSlots;
    }
  }

  private updateState(draftState: IDraftState) {
    this.draftState = draftState;
    if (draftState.phase === DraftPhase.WAITING) {
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
      this.team1Status = Math.max(draftState.team1BonusTime, 0).toString();
      this.team2Status = Math.max(draftState.team2BonusTime, 0).toString();
    }
    this.changeRef.detectChanges();
  }

  public get pickTime(): string {
    if (this.draftState) {
      return Math.max(this.draftState.pickTime, 0).toString();
    }
    return '';
  }

  public getPick(team: number, pickId: number): HeroData {
    if (!this.heroes || !this.draftConfig || !this.draftState) {
      return null;
    }
    let picks = this.draftState.picks || [];
    let pickedHeroId: string;

    let pickSlots: number[] = this.teamPickSlots[team];

    pickedHeroId = picks[pickSlots[pickId]];
    return this.getHeroById(pickedHeroId);
  }

  public getBan(team: number, pickId: number): HeroData {
    if (!this.heroes || !this.draftConfig || !this.draftState) {
      return null;
    }
    let picks = this.draftState.picks || [];
    let pickedHeroId: string;

    let banSlots: number[] = this.teamBanSlots[team];

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

  public pick() {
    this.draftService.pickHero(this.selectedHero.id, this.draftToken, this.teamToken);
  }

  public get currentPick(): number {
    if (!this.draftState || this.draftState.phase !== DraftPhase.PICKING) {
      return -1;
    }
    return this.draftState.picks ? this.draftState.picks.length : -1;
  }

  public get currentTeam(): number {
    if (this.currentPick !== -1) {
      return DraftComponent.firstSlots.indexOf(this.currentPick) !== -1 ?
        (this.draftConfig.firstPick === 1 ? 0 : 1) :
        (this.draftConfig.firstPick === 1 ? 1 : 0);
    }
    return -1;
  }

  public get isBan(): boolean {
    if (this.draftState && this.draftState.phase === DraftPhase.PICKING) {
      return this.teamBanSlots[0].indexOf(this.currentPick) !== -1 || this.teamBanSlots[1].indexOf(this.currentPick) !== -1;
    }
    return false;
  }

  public get draftStatus(): string {
    if (this.draftState) {
      if (this.draftState.phase === DraftPhase.WAITING) {
        if (this.draftConfig.firstPick === 1) {
          return 'Blue team picks first';
        } else {
          return 'Red team picks first';
        }
      } else if (this.draftState.phase === DraftPhase.FINISHED) {
        return 'Draft Completed';
      } else {
        if (this.currentTeam === 0) {
          return this.isBan ? 'Blue team banning' : 'Blue team picking';
        } else {
          return this.isBan ? 'Red team banning' : 'Red team picking';
        }
      }
    }
    return '';
  }

}
