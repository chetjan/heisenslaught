import { Component, OnInit, Input } from '@angular/core';
import { HeroData } from '../services/hero';

@Component({
  selector: 'hero-pick',
  template: `
    <div class="bg"></div>
     <svg viewBox="0 0 100 100" [hidden]="!hero">
        <defs>
          <mask id="hero-pick-mask">
            <path d="M 52 4 L 100 28 L 100 78 L 52 101 L 5 78 L 5 28 z"
              fill="white" />
          </mask>
        </defs>
        <image 
          [attr.xlink:href]="hero?.iconSmall" 
          width="100" height="100" mask="url(#hero-pick-mask)"
        />
      </svg>
    <div class="border"></div>
  `,
  styleUrls: ['./hero-pick.component.css']
})
export class HeroPickComponent implements OnInit {

  @Input()
  public hero: HeroData;

  constructor() { }

  ngOnInit() {
  }

}
