import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { SignalRHub, SignalRConnectionService, HubEventHandler, HubMethodHandler, SignalRConnectionState }
    from '../../../services/signalr/signalr-hub';
import { IDraftHubServerProxy } from './types/draft-hub-proxy';
import { IDraftConfigAdminDTO, IDraftConfigDTO, ICreateDraftDTO } from './types/draft-config';
import { IDraftState } from './types/draft-state';
import { IDraftUser } from './types/draft-users';

export { SignalRConnectionState } from '../../../services/signalr/signalr-hub';
export * from './types/draft-state';
export * from './types/draft-users';
export * from './types/draft-config';

@Injectable()
export class DraftService extends SignalRHub<IDraftHubServerProxy>{
    private _draftTokens: string[];
    private _connectedUsersSubject: Subject<IDraftUser[]> = new Subject<IDraftUser[]>();
    private _connectedUsers: IDraftUser[] = [];
    private _draftConfig: IDraftConfigDTO;
    private _draftState: IDraftState;
    private _reconnecting: boolean = false;

    @HubEventHandler()
    private updateConfig: Observable<IDraftConfigDTO>;

    @HubEventHandler()
    private updateDraftState: Observable<IDraftState>;


    constructor(signalRConnectionService: SignalRConnectionService) {
        super(signalRConnectionService, 'draftHub');
    }

    public connectToDraft(draftToken: string, userToken?: string): Promise<IDraftConfigDTO> {
        this._draftTokens = [draftToken, userToken];
        this.connect();
        let p = this.server.connectToDraft(draftToken, userToken);
        p.then((config) => {
            this._draftConfig = config;
            this._draftState = config.state;
            this.emitClientEvent('updateConfig', config);
            this.emitClientEvent('updateDraftState', config.state);
        });
        return p;
    }

    public createDraft(createCfg: ICreateDraftDTO): Promise<IDraftConfigAdminDTO> {
        let p = this.server.createDraft(createCfg);
        p.then((config) => {
            this._draftConfig = config;
            this._draftState = config.state;
        });
        return p;
    }

    // TODO: refactor for consistancy - resetDraft vs restartDraft
    public resetDraft(draftToken: string, adminToken: string): Promise<IDraftConfigAdminDTO> {
        let p = this.server.restartDraft(draftToken, adminToken);
        p.then((config) => {
            this._draftConfig = config;
            this._draftState = config.state;
        });
        return p;
    }

    public closeDraft(draftToken: string, adminToken: string): Promise<void> {
        return this.server.closeDraft(draftToken, adminToken);
    }

    public setReady(draftToken: string, teamToken: string): Promise<boolean> {
        return this.server.setReady(draftToken, teamToken);
    }

    public pickHero(heroId: string, draftToken: string, teamToken: string): Promise<boolean> {
        return this.server.pickHero(heroId, draftToken, teamToken);
    }

    public get draftConfigObservable(): Observable<IDraftConfigDTO> {
        return this.updateConfig;
    }

    public get draftStateObservable(): Observable<IDraftState> {
        return this.updateDraftState;
    }

    public get connectedUserObsevable(): Observable<IDraftUser[]> {
        return this._connectedUsersSubject.asObservable();
    }

    public get connectedUsers(): IDraftUser[] {
        return this._connectedUsers;
    }

    public disconnect() {
        this._draftTokens = null;
        super.disconnect();
    }

    @HubMethodHandler('SetConnectedUsers')
    protected setConnectedUsers(users: IDraftUser[]): void {
        this._connectedUsers = users;
        this.sortUsers();
        this._connectedUsersSubject.next(users);
    }

    @HubMethodHandler('OnUserJoined')
    protected onUserJoined(user: IDraftUser): void {
        let match = this._connectedUsers.find((item) => {
            return item.id === user.id;
        });
        if (match) {
            Object.assign(match, user);
        } else {
            this._connectedUsers.push(user);
        }
        this.sortUsers();
        this._connectedUsersSubject.next(this.connectedUsers);
    }

    @HubMethodHandler('OnUserStatusUpdate')
    protected onUserStatusUpdate(user: IDraftUser): void {
        let match = this._connectedUsers.find((item) => {
            return item.id === user.id;
        });
        if (match) {
            Object.assign(match, user);
        } else {
            this._connectedUsers.push(user);
        }
        this.sortUsers();
        this._connectedUsersSubject.next(this.connectedUsers);
    }

    @HubMethodHandler('OnUserLeft')
    protected onUserLeft(user: IDraftUser): void {
        let match = this._connectedUsers.find((item) => {
            return item.id === user.id;
        });
        if (match) {
            let idx = this._connectedUsers.indexOf(match);
            this._connectedUsers.splice(idx, 1);
            this.sortUsers();
            this._connectedUsersSubject.next(this.connectedUsers);
        }
    }

    protected stateChange(newState: SignalRConnectionState, oldState: SignalRConnectionState): void {
        super.stateChange(newState, oldState);
        if (newState !== oldState) {
            switch (newState) {
                case SignalRConnectionState.DISCONNECTED:
                    if (this._draftTokens) {
                        this._reconnecting = true;
                        setTimeout(() => {
                            console.log('Attemping to reconnect...');
                            this.reconnect();
                        }, 10000);
                    }
                    break;
                case SignalRConnectionState.RECONNECTING:
                    this._reconnecting = true;
                    break;
                case SignalRConnectionState.CONNECTED:
                    if (this._reconnecting && this._draftTokens) {
                        console.log('Reconnected.');
                        this.connectToDraft(this._draftTokens[0], this._draftTokens[1]);
                    }
                    this._reconnecting = false;
                    break;
            }
        }
    }

    private sortUsers() {
        this._connectedUsers.sort((a, b) => {
            let aT = a.connectionTypes & 1;
            let bT = b.connectionTypes & 1;
            if (aT > bT) {
                return -1;
            } if (aT < bT) {
                return 1;
            }

            aT = a.connectionTypes & 2;
            bT = b.connectionTypes & 2;
            if (aT > bT) {
                return -1;
            } if (aT < bT) {
                return 1;
            }

            aT = a.connectionTypes & 4;
            bT = b.connectionTypes & 4;
            if (aT > bT) {
                return -1;
            } if (aT < bT) {
                return 1;
            }

            return a.name.localeCompare(b.name);
        });
    }
}
