import { Component, OnInit, Input, OnDestroy } from '@angular/core';

import { HeroData } from '../../../heroes-data-service/heroes-data-service.module';


@Component({
  selector: 'hero-icon',
  template: `
    <div #ttAttach>
      <svg viewBox="0 0 100 100">
        <image [attr.xlink:href]="hero.iconSmall" width="100" height="100"></image>
      </svg>
      <div class="picked-icon"></div>
    </div>
    <img class="select-border" src="http://us.battle.net/heroes/static/images/heroes/skin_hover.png"/>
    <app-tooltip>
      <hero-info-card [hero]="hero">
        <div class="show-more-info" *ngIf="!isAltDown">
          <span class="bracket">[ </span><span class="key">Alt</span><span class="bracket"> ]</span> for more info.
        </div>
        <div class="more-info" *ngIf="isAltDown">
          <div><label>Roles: </label>{{hero?.roles.join(', ')}}</div>
        </div>
      </hero-info-card>
    </app-tooltip>
  `,
  styleUrls: ['./hero-icon.component.scss']
})
export class HeroIconComponent implements OnInit, OnDestroy {
  private static _keyDownListener: EventListener;
  private static _keyUpListener: EventListener;
  private static _isAltDown: boolean;
  private static _instanceCount: number = 0;

  @Input()
  public hero: HeroData;

  public get isAltDown(): boolean {
    return HeroIconComponent._isAltDown;
  }

  constructor() {
    this.initEvents();
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.destEvents();
  }

  private initEvents(): void {
    if (HeroIconComponent._instanceCount === 0) {
      HeroIconComponent._keyDownListener = (evt: KeyboardEvent) => {
        if (evt.altKey) {
          HeroIconComponent._isAltDown = true;
          // for some reason if you don't call prevent default the event only fires every second time
          event.preventDefault();
          window.addEventListener('keyup', HeroIconComponent._keyUpListener);
          window.removeEventListener('keydown', HeroIconComponent._keyDownListener);
        }
      };
      HeroIconComponent._keyUpListener = (evt: KeyboardEvent) => {
        if (!evt.altKey) {
          HeroIconComponent._isAltDown = false;
          window.addEventListener('keydown', HeroIconComponent._keyDownListener);
          window.removeEventListener('keyup', HeroIconComponent._keyUpListener);
        }
      };
      window.addEventListener('keydown', HeroIconComponent._keyDownListener);
    }
    HeroIconComponent._instanceCount++;
  }

  private destEvents(): void {
    HeroIconComponent._instanceCount--;
    if (HeroIconComponent._instanceCount === 0) {
      window.removeEventListener('keydown', HeroIconComponent._keyDownListener);
      window.removeEventListener('keyup', HeroIconComponent._keyUpListener);
    }
  }



}
