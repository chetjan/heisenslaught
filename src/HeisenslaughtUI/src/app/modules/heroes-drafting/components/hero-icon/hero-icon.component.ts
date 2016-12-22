import { Component, OnInit, Input, HostBinding } from '@angular/core';

import { HeroData } from '../../../heroes-data-service/heroes-data-service.module';


@Component({
  selector: 'hero-icon',
  template: `
    <div>
      <svg viewBox="0 0 100 100">
        <image [attr.xlink:href]="hero.iconSmall" width="100" height="100"></image>
      </svg>
      <div class="picked-icon"></div>
    </div>
    <img src="http://us.battle.net/heroes/static/images/heroes/skin_hover.png"/>
  `,
  styleUrls: ['./hero-icon.component.scss']
})
export class HeroIconComponent implements OnInit {

  @Input()
  public hero: HeroData;

  constructor() { }

  @HostBinding('attr.title')
  public get heroName(): string {
    return this.hero ? this.hero.name : '';
  }

  ngOnInit() {
  }

}
