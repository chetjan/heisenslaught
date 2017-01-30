import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { DraftHubService, ICreateDraftDTO, IDraftConfigAdminDTO, DraftPhase } from '../../../heroes-draft-service/services/draft.service';
import { HeroesService, IMapData, HeroData } from '../../../heroes-data-service/services/heroes.service';


interface ConfigPreset {
  name: string;
  config: ICreateDraftDTO;
}

@Component({
  selector: 'draft-config-screen',
  templateUrl: './draft-config-screen.component.html',
  styleUrls: ['./draft-config-screen.component.scss']
})
export class DraftConfigScreenComponent implements OnInit, OnDestroy {
  private draftToken: string;
  private adminToken: string;
  private loadedHeroes: boolean;
  private loadedMaps: boolean;
  private loadedConfig: boolean;
  private stateSubscription: Subscription;

  private baseConfig: ICreateDraftDTO = {
    firstPick: 0,
    bankTime: true,
    team1Name: 'Blue Team',
    team2Name: 'Red Team',
    pickTime: 45,
    bonusTime: 90,
    disabledHeroes: undefined,
    map: undefined
  };

  public config: ICreateDraftDTO = <ICreateDraftDTO>{};
  public currentConfig: IDraftConfigAdminDTO;
  public maps: IMapData[];
  public heroes: HeroData[];
  public createError: string;
  public selectedPreset: ConfigPreset;

  public presets: ConfigPreset[] = [
    {
      name: 'Default',
      config: this.baseConfig
    },
    {
      name: 'Extended',
      config: <ICreateDraftDTO>{
        firstPick: 0,
        bankTime: true,
        pickTime: 60,
        bonusTime: 180
      }
    },
    {
      name: 'Blizzard',
      config: <ICreateDraftDTO>{
        firstPick: 0,
        bankTime: false,
        pickTime: 45,
        bonusTime: 0
      }
    }, {
      name: 'Blizzard Tournament',
      config: <ICreateDraftDTO>{
        firstPick: 0,
        bankTime: false,
        pickTime: 60,
        bonusTime: 0
      }
    },
    {
      name: 'Tournament',
      config: <ICreateDraftDTO>{
        firstPick: 0,
        bankTime: false,
        pickTime: 30,
        bonusTime: 60
      }
    },

    {
      name: 'ARAM',
      config: <ICreateDraftDTO>{
        firstPick: 0,
        bankTime: false,
        pickTime: -1,
        bonusTime: -1
      }
    }
  ];


  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private heroesService: HeroesService,
    private draftService: DraftHubService,
    private changeRef: ChangeDetectorRef
  ) {
    try {
      this.draftToken = this.route.snapshot.params['id'];
      this.adminToken = this.route.snapshot.params['adminToken'];

      if (this.draftToken && this.adminToken) {
        this.initConfigDraft();
      } else {
        this.initCreateDraft();
      }
    } catch (e) {
      console.error(e);
    }
  }

  public async ngOnInit() {
    [this.maps, this.heroes] = await Promise.all<any>([
      this.heroesService.getMaps(),
      this.heroesService.getHeroes()
    ]);
    this.loadedMaps = true;
    this.loadedHeroes = true;
  }

  public ngOnDestroy() {
    if (this.stateSubscription) {
      this.stateSubscription.unsubscribe();
    }
    this.draftService.disconnect();
  }

  public get loaded(): boolean {
    return this.loadedConfig && this.loadedHeroes && this.loadedMaps;
  }

  private initCreateDraft(): void {
    this.selectedPreset = this.presets[0];
    this.loadPreset(this.selectedPreset);
    this.config.disabledHeroes = [];
    this.loadedConfig = true;
  }

  private composeConfig(base: ICreateDraftDTO, add: ICreateDraftDTO): ICreateDraftDTO {
    base = Object.assign({}, base);
    for (let prop in add) {
      if (add[prop] !== undefined) {
        base[prop] = add[prop];
      }
    }
    return base;
  }

  public loadPreset(preset: ConfigPreset) {
    this.config = this.composeConfig(this.config, preset.config);
  }

  private async initConfigDraft() {
    let config = await this.draftService.connectToDraft(this.draftToken, this.adminToken);
    this.currentConfig = <IDraftConfigAdminDTO>config;
    this.stateSubscription = this.draftService.draftStateObservable.subscribe((state) => {
      this.currentConfig.state = state;
      this.changeRef.detectChanges();
    });
    this.loadedConfig = true;
  }

  public get isDraftComplete(): boolean {
    if (this.currentConfig && this.currentConfig.state) {
      return this.currentConfig.state.phase === DraftPhase.FINISHED;
    }
    return false;
  }

  public get isDraftWaiting(): boolean {
    if (this.currentConfig && this.currentConfig.state) {
      return this.currentConfig.state.phase === DraftPhase.WAITING;
    }
    return false;
  }

  public async createDraft() {
    this.createError = undefined;
    let cfg = await this.draftService.createDraft(this.config);
    this.router.navigate(['draft/config', cfg.draftToken, cfg.adminToken]);
  }

  public async resetDraft() {
    this.createError = undefined;
    this.currentConfig = await this.draftService.resetDraft(this.draftToken, this.adminToken);
  }

  public async closeDraft() {
    this.createError = undefined;
    await this.draftService.closeDraft(this.draftToken, this.adminToken);
  }

  public getMapName(): string {
    let mapName = '';
    if (this.currentConfig && this.maps) {
      let map = this.maps.find((value) => {
        return value.id === this.currentConfig.map;
      });
      if (map) {
        mapName = map.name;
      }
    }
    return mapName;
  }

  public getStatus(): string {
    return 'WAITING';
  }

  public getFirstPick(): string {
    let str = '';
    if (this.currentConfig) {
      str = this.currentConfig.firstPick === 1 ? this.currentConfig.team1Name : this.currentConfig.team2Name;
      if (this.currentConfig.wasFirstPickRandom) {
        str += ' (Random)';
      }
    }
    return str;
  }

  public getPhase(): string {
    let str = '';
    if (this.currentConfig) {
      switch (this.currentConfig.state.phase) {
        case DraftPhase.WAITING:
          str = 'Waiting';
          break;
        case DraftPhase.PICKING:
          str = 'Picking';
          break;
        case DraftPhase.FINISHED:
          str = 'Completed';
          break;
      }
    }
    return str;
  }

  public getLink(draftToken: string, teamToken: string): string {
    let url = window.location.protocol + '//';
    url += window.location.host + '/draft/' + draftToken;
    if (teamToken) {
      url += '/' + teamToken;
    }
    return url;
  }

  public copyLink(input: HTMLInputElement): void {
    try {
      input.select();
      document.execCommand('copy');
    } catch (e) { }
  }
}
