import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';

import { DraftService, HubConnectionState, IDraftUser } from '../../../heroes-draft-service/heroes-draft-service.module';


@Component({
  selector: 'draft-connection-status',
  templateUrl: './draft-connection-status.component.html',
  styleUrls: ['./draft-connection-status.component.scss']
})
export class DraftConnectionStatusComponent implements OnInit, OnDestroy {
  private connectionStateSub: Subscription;
  private userSubscription: Subscription;

  public connectionIcon: string = 'warning';
  public connectionStatus: string = 'disconnected';
  public connectionCount: number = 0;
  public connectedUsers: IDraftUser[] = [];

  constructor(
    private draftService: DraftService,
    private changeRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.updateConnectionState(this.draftService.connectionState);
    this.updateUserConnections(this.draftService.connectedUsers);

    this.connectionStateSub = this.draftService.connectionStateObservable.subscribe((state) => {
      this.updateConnectionState(state);
    });
    this.userSubscription = this.draftService.connectedUserObsevable.subscribe((users) => {
      this.updateUserConnections(users);
    });
  }

  ngOnDestroy() {
    this.connectionStateSub.unsubscribe();
    this.userSubscription.unsubscribe();
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

  private updateUserConnections(users: IDraftUser[]) {
    if (users) {
      this.connectionCount = users.length;
      this.connectedUsers = users;
      this.changeRef.detectChanges();
    }
  }

  public status(user: IDraftUser): any {
    return {
      d1: (user.connectionTypes & 1) === 1,
      d2: (user.connectionTypes & 2) === 2,
      obs: (user.connectionTypes & 4) === 4,
      admin: (user.connectionTypes & 8) === 8
    };
  }
}
