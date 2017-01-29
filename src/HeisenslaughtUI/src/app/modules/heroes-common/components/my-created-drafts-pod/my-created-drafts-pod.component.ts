import { Component, OnInit } from '@angular/core';
import { DraftService } from '../../../heroes-draft-service/services/draft-api.service';
import { HeroesService, IMapData } from '../../../heroes-data-service/services/heroes.service';

@Component({
  selector: 'my-created-drafts-pod',
  templateUrl: './my-created-drafts-pod.component.html',
  styleUrls: ['./my-created-drafts-pod.component.scss']
})
export class MyCreatedDraftsPodComponent implements OnInit {

  private mapData: IMapData[];
  public recentDrafts: any[];


  constructor(
    private draftService: DraftService,
    private heroesService: HeroesService
  ) {
    draftService.getMyRecentlyCreatedDrafts().subscribe((recentDrafts) => {
      this.recentDrafts = recentDrafts;
    });
  }

  public getMapName(mapId: string): string {
    if (this.mapData && this.mapData.length) {
      let map = this.mapData.find((mapItem) => {
        return mapItem.id === mapId;
      });
      return map ? map.name : 'Unknown Map';
    }
    return 'Unknown Map';
  }

  async ngOnInit() {
    this.mapData = await this.heroesService.getMaps();
  }
}
