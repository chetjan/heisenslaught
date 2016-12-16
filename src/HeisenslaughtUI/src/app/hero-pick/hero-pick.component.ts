import { Component, Input } from '@angular/core';
import { HeroData } from '../services/hero';


@Component({
  selector: 'hero-pick',
  templateUrl: './hero-pick.component.html',
  styleUrls: ['./hero-pick.component.scss']
})
export class HeroPickComponent {
  @Input()
  public hero: HeroData;
}
