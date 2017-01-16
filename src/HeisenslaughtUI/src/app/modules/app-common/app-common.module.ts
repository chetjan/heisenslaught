import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@angular/material';
import { HtmlTooltipComponent } from './components/html-tooltip/html-tooltip.component';
import { NavigationComponent } from './components/navigation/navigation.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule
  ],
  declarations: [
    HtmlTooltipComponent,
    NavigationComponent
  ],
  exports: [
    HtmlTooltipComponent,
    NavigationComponent
  ]
})
export class AppCommonModule { }
