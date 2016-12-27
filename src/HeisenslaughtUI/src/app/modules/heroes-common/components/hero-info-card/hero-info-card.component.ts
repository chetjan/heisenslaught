import { Component, OnInit, Input } from '@angular/core';
import { HeroData } from '../../../heroes-data-service/services/heroes.service';

@Component({
  selector: 'hero-info-card',
  templateUrl: './hero-info-card.component.html',
  styleUrls: ['./hero-info-card.component.scss']
})
export class HeroInfoCardComponent implements OnInit {

  @Input()
  public hero: HeroData;

  constructor() { }

  ngOnInit() {
  }

}
