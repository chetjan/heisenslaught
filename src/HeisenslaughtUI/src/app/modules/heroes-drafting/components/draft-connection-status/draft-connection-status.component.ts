import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DraftService, HubConnectionState } from '../../../heroes-draft-service/heroes-draft-service.module';
@Component({
  selector: 'draft-connection-status',
  templateUrl: './draft-connection-status.component.html',
  styleUrls: ['./draft-connection-status.component.scss']
})
export class DraftConnectionStatusComponent implements OnInit {

  public connectionIcon: string = 'warning';
  public connectionStatus: string = 'disconnected';
  constructor(
    private draftService: DraftService,
    private changeRef: ChangeDetectorRef
  ) { }

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

  ngOnInit() {
    this.draftService.getConnectionState().subscribe((state) => {
      this.updateConnectionState(state);
    });
  }

}
