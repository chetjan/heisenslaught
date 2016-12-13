import { Component, OnInit } from '@angular/core';
import { DraftService, ICreateDraftData, ICreateDraftResult, DraftPhase } from '../services/draft.service';
import { HeroesService, IMapData, HeroData } from '../services/heroes.service';


@Component({
  selector: 'draft-config-screen',
  templateUrl: './draft-config-screen.component.html',
  styleUrls: ['./draft-config-screen.component.css']
})
export class DraftConfigScreenComponent implements OnInit {
  public config: ICreateDraftData = <ICreateDraftData>{};
  public currentConfig: ICreateDraftResult;
  public maps: IMapData[];
  public heroes: HeroData[];
  public loaded: boolean = false;

  public createError: string;

  constructor(
    private heroesService: HeroesService,
    private draftService: DraftService
  ) {

    heroesService.getMaps().subscribe((maps: IMapData[]) => {
      this.maps = maps;
    });

    heroesService.getHeroes().subscribe((heroes: HeroData[]) => {
      this.heroes = heroes;
    });

    draftService.getCurrentAdminConfig().then((cfg) => {
      if (cfg) {
        if (cfg.state.phase === DraftPhase.FINISHED) {
          this.config = cfg;
          if (cfg.randomFirstPick) {
            this.config.firstPick = 0;
          }
          this.currentConfig = null;
        } else {
          this.currentConfig = cfg;
        }
      } else {
        this.config = <ICreateDraftData>{
          firstPick: 0,
          bankTime: true,
          team1Name: 'Team 1',
          team2Name: 'Team 2',
          pickTime: 60,
          bonusTime: 180
        };
      }
      this.loaded = true;
    }, (err) => {
      this.createError = err ? err.toString() : 'Server Error';
    });
  }

  ngOnInit() {
  }

  public createDraft() {
    this.createError = undefined;

    this.draftService.createDraft(this.config).then((cfg) => {
      this.currentConfig = cfg;
    }, (err) => {
      this.createError = err ? err.toString() : 'Server Error';
    });
  }


  public resetDraft() {
    this.createError = undefined;
    this.draftService.resetDraft().then((cfg) => {
      this.currentConfig = cfg;
    }, (err) => {
      this.createError = err ? err.toString() : 'Server Error';
    });
  }

  public closeDraft() {
    this.createError = undefined;
    this.draftService.closeDraft().then((cfg) => {
      this.currentConfig = null;
      this.config = cfg;
      if (cfg.randomFirstPick) {
        this.config.firstPick = 0;
      }
    }, (err) => {
      this.createError = err ? err.toString() : 'Server Error';
    });
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

  public getFirstPick(): string {
    let str = '';
    if (this.currentConfig) {
      str = this.currentConfig.firstPick === 1 ? this.currentConfig.team1Name : this.currentConfig.team2Name;
      if (this.currentConfig.randomFirstPick) {
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
          str = 'Waiting for teams to be ready';
          break;
      }
    }
    return str;
  }
}
