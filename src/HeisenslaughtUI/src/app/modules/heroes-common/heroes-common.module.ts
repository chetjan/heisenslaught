import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeroesButtonComponent } from './components/heroes-button/heroes-button.component';
import { HeroInfoCardComponent } from './components/hero-info-card/hero-info-card.component';


@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    HeroesButtonComponent,
    HeroInfoCardComponent
  ],
  exports: [
    HeroesButtonComponent,
    HeroInfoCardComponent
  ]
})
export class HeroesCommonModule { }
