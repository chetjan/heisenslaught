import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeroesButtonComponent } from './components/heroes-button/heroes-button.component';


@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    HeroesButtonComponent
  ],
  exports: [
    HeroesButtonComponent
  ]
})
export class HeroesCommonModule { }
