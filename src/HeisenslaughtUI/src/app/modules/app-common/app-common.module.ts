import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@angular/material';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HtmlTooltipComponent } from './components/html-tooltip/html-tooltip.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { ContentPodComponent } from './components/content-pod/content-pod.component';
import { BreadCrumbComponent } from './components/bread-crumb/bread-crumb.component';
import { PageComponent } from './components/page/page.component';


@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    FlexLayoutModule
  ],
  declarations: [
    HtmlTooltipComponent,
    NavigationComponent,
    ContentPodComponent,
    PageComponent,
    BreadCrumbComponent
  ],
  exports: [
    HtmlTooltipComponent,
    NavigationComponent,
    ContentPodComponent,
    PageComponent,
    BreadCrumbComponent
  ]
})
export class AppCommonModule { }
