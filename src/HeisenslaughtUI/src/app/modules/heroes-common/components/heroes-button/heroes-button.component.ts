import { Component, Input } from '@angular/core';

@Component({
  selector: 'heroes-button',
  templateUrl: './heroes-button.component.html',
  styleUrls: ['./heroes-button.component.scss']
})
export class HeroesButtonComponent {

  @Input()
  public label: string;

  @Input()
  public disabled: boolean;

  constructor() { }

}
