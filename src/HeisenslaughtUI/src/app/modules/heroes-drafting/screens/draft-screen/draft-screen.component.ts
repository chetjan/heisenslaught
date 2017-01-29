import { Component, ChangeDetectorRef, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

import { DraftHubService, IDraftConfigDTO, IDraftState, DraftPhase, IDraftConfigDrafterDTO }
  from '../../../heroes-draft-service/services/draft.service';
import { HeroesService, HeroData, IMapData } from '../../../heroes-data-service/services/heroes.service';


@Component({
  selector: 'draft-screen',
  templateUrl: './draft-screen.component.html',
  styleUrls: ['./draft-screen.component.scss']
})
export class DraftScreenComponent implements OnInit, OnDestroy {

  private static firstSlots: number[] = [0, 2, 5, 6, 8, 11, 12];
  private static secondSlots: number[] = [1, 3, 4, 7, 9, 10, 13];
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
  private teamBanSlots: Array<number[]> = [];

  private configSubscription: Subscription;
  private stateSubscription: Subscription;

  public teamPickSlots: Array<number[]> = [];
  public selectedHero: any;
  public draftConfig: IDraftConfigDTO;
  public draftState: IDraftState;
  public team1Status: string;
  public team2Status: string;


  constructor(
    private draftService: DraftHubService,
    private heroesService: HeroesService,
    private route: ActivatedRoute,
    private changeRef: ChangeDetectorRef
  ) {
    
    this.draftToken = this.route.snapshot.params['id'];
    this.teamToken = this.route.snapshot.params['team'];

    this.draftService.connectToDraft(this.draftToken, this.teamToken).then((config) => {
      this.draftConfig = config;
      this.team = (<IDraftConfigDrafterDTO>config).team;
      this.configSlots();
      this.updateState(config.state);
      this.configSubscription = this.draftService.draftConfigObservable.subscribe((cfg) => {
        this.draftConfig = cfg;
        this.configSlots();
        this.updateState(cfg.state);
      });
      this.stateSubscription = this.draftService.draftStateObservable.subscribe((state) => {
        this.updateState(state);
      });
    }, (err) => {
      console.log('connect error', err);
    });
  }

  public async ngOnInit() {
    this.heroes = await this.heroesService.getHeroes();
    this.maps = await this.heroesService.getMaps();
  }

  public ngOnDestroy() {
    this.stateSubscription.unsubscribe();
    this.configSubscription.unsubscribe();
    this.draftService.disconnect();
  }
  private configSlots(): void {
    if (this.draftConfig.firstPick === 1) {
      this.teamSlots[0] = DraftScreenComponent.firstSlots;
      this.teamSlots[1] = DraftScreenComponent.secondSlots;
      this.teamPickSlots[0] = DraftScreenComponent.firstPickSlots;
      this.teamPickSlots[1] = DraftScreenComponent.secondPickSlots;
      this.teamBanSlots[0] = DraftScreenComponent.firstBanSlots;
      this.teamBanSlots[1] = DraftScreenComponent.secondBanSlots;
    } else {
      this.teamSlots[0] = DraftScreenComponent.secondSlots;
      this.teamSlots[1] = DraftScreenComponent.firstSlots;
      this.teamPickSlots[0] = DraftScreenComponent.secondPickSlots;
      this.teamPickSlots[1] = DraftScreenComponent.firstPickSlots;
      this.teamBanSlots[0] = DraftScreenComponent.secondBanSlots;
      this.teamBanSlots[1] = DraftScreenComponent.firstBanSlots;
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
    } else {
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
    this.selectedHero = null;
  }

  public get currentPick(): number {
    if (!this.draftState || this.draftState.phase !== DraftPhase.PICKING) {
      return -1;
    }
    return this.draftState.picks ? this.draftState.picks.length : -1;
  }

  public get currentTeam(): number {
    if (this.currentPick !== -1) {
      return DraftScreenComponent.firstSlots.indexOf(this.currentPick) !== -1 ?
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

  public get shouldShowSelect(): boolean {
    if (this.draftState && this.draftState.phase === DraftPhase.PICKING) {
      if (this.draftConfig.pickTime === -1) {
        return false;
      }

      if (this.team) {
        return true;
      }
    }
    return false;
  }

  public isHeroPicked(heroId: string) {
    if (this.draftState && this.draftState.picks) {
      if (this.draftState.picks.lastIndexOf(heroId) !== -1) {
        return true;
      }
      if (heroId === 'cho' || heroId === 'gall') {
        return this.draftState.picks.lastIndexOf(heroId === 'cho' ? 'gall' : 'cho') !== -1;
      }

    }
    return false;
  }

  public get canPick(): boolean {
    if (this.draftState && this.draftState.phase === DraftPhase.PICKING) {
      if (this.team === (this.currentTeam + 1) && this.selectedHero) {
        return !this.isHeroPicked(this.selectedHero.id);
      }
    }
    return false;
  }

}
