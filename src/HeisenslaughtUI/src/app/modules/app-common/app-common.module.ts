import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@angular/material';
import { HtmlTooltipComponent } from './components/html-tooltip/html-tooltip.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { ContentPodComponent } from './components/content-pod/content-pod.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule
  ],
  declarations: [
    HtmlTooltipComponent,
    NavigationComponent,
    ContentPodComponent
  ],
  exports: [
    HtmlTooltipComponent,
    NavigationComponent,
    ContentPodComponent
  ]
})
export class AppCommonModule { }
