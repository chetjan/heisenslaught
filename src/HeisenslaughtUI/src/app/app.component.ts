import { Component, ChangeDetectorRef } from '@angular/core';
import { ChannelService } from './signalr/channel.service';
import { HeroesService } from './services/heroes.service';

interface IDraftState {
  TimeTeam0: number;
  TimeTeam1: number;
  TimeBonus: number;
  CurrentState: string;
  CurrentAction: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app works!';

  private draftState: IDraftState;
  private heroes: any[];

  constructor(private chanServe: ChannelService, private ref: ChangeDetectorRef, private heroesService: HeroesService) {
    heroesService.getHeroes().subscribe((data) => {
      this.heroes = data;
    });

    chanServe.registerMethod('updateDraftState', (draftState: IDraftState) => {
      //  console.log('called method', this);
      this.draftState = draftState;
      this.ref.detectChanges();
    });
  //  chanServe.connect();
  }
}
