import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HtmlTooltipComponent } from './components/html-tooltip/html-tooltip.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [HtmlTooltipComponent],
  exports:[HtmlTooltipComponent]
})
export class AppCommonModule { }
