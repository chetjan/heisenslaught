import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';

import { DraftService, HubConnectionState, IDraftState } from '../../../heroes-draft-service/heroes-draft-service.module';


@Component({
  selector: 'draft-connection-status',
  templateUrl: './draft-connection-status.component.html',
  styleUrls: ['./draft-connection-status.component.scss']
})
export class DraftConnectionStatusComponent implements OnInit, OnDestroy {
  private connectionStateSub: Subscription;
  private stateSubscription: Subscription;

  public connectionIcon: string = 'warning';
  public connectionStatus: string = 'disconnected';
  public connectionCount: number = 0;


  constructor(
    private draftService: DraftService,
    private changeRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.updateConnectionState(this.draftService.connectionState);
    this.updateDraftState(this.draftService.draftState);

    this.connectionStateSub = this.draftService.connectionStateObservable.subscribe((state) => {
      this.updateConnectionState(state);
    });
    this.stateSubscription = this.draftService.draftStateObservable.subscribe((state) => {
      this.updateDraftState(state);
    });
  }

  ngOnDestroy() {
    this.connectionStateSub.unsubscribe();
    this.stateSubscription.unsubscribe();
  }

  private updateConnectionState(state: HubConnectionState): void {
    try {
      this.connectionStatus = HubConnectionState[state].toLowerCase();
      switch (state) {
        case HubConnectionState.CONNECTED:
        case HubConnectionState.CONNECTED_SLOW:
          this.connectionIcon = 'wifi';
          break;
        case HubConnectionState.CONNECTING:
          this.connectionIcon = 'autorenew';
          break;
        case HubConnectionState.DISCONNECTED:
          this.connectionIcon = 'warning';
          break;
        case HubConnectionState.RECONNECTING:
          this.connectionIcon = 'autorenew';
          break;
      }
      this.changeRef.detectChanges();
    } catch (e) { }
  }

  private updateDraftState(state: IDraftState) {
    if (state) {
      this.connectionCount = state.connectionCount;
      this.changeRef.detectChanges();
    }
  }

}
