import { NgModule } from '@angular/core';
import { DraftService } from './services/draft.service';

export * from './services/draft.service';


@NgModule({
  providers: [
    DraftService
  ]
})
export class HeroesDraftServiceModule { }
