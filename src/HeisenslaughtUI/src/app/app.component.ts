import { Component } from '@angular/core';


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
  styleUrls: ['./app.component.scss']
})
export class AppComponent { }
