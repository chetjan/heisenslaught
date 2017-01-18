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

    @HubMethodHandler('SetConnectedUsers')
    protected setConnectedUsers(users: IDraftUser[]): void {

    }

    @HubMethodHandler('OnUserJoined')
    protected onUserJoined(user: IDraftUser): void {

    }

    @HubMethodHandler('OnUserStatusUpdate')
    protected onUserStatusUpdate(user: IDraftUser): void {

    }

    @HubMethodHandler('OnUserLeft')
    protected onUserLeft(user: IDraftUser): void {

    }

    protected stateChange(newState: SignalRConnectionState, oldState: SignalRConnectionState): void {
        super.stateChange(newState, oldState);
        console.log('State changed from ' + SignalRConnectionState[oldState] + ' to ' + SignalRConnectionState[newState]);
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



/*
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs/Rx';

import { IDraftConfigAdminDTO, IDraftConfigDTO, ICreateDraftDTO } from './types/draft-config';
import { IDraftState } from './types/draft-state';
import { IDraftUser } from './types/draft-users';
import { IDraftHubProxy } from './types/draft-hub-proxy';
import { HubConnectionState } from './types/hub-connection-sate';

export { ICreateDraftDTO, IDraftConfigAdminDTO, IDraftConfigDTO, IDraftConfigDrafterDTO } from './types/draft-config';
export { IDraftState, DraftPhase } from './types/draft-state';
export { IDraftUser } from './types/draft-users';
export { HubConnectionState } from './types/hub-connection-sate';


@Injectable()
export class DraftService {
    private hub: IDraftHubProxy;
    private connectPromise: Promise<any>;
    private connectionArguments: string[];
    private _connectionSubject: Subject<HubConnectionState> = new Subject<HubConnectionState>();
    private _draftConfigSubject: Subject<IDraftConfigDTO> = new Subject<IDraftConfigDTO>();
    private _draftStateSubject: Subject<IDraftState> = new Subject<IDraftState>();
    private _connectedUsersSubject: Subject<IDraftUser[]> = new Subject<IDraftUser[]>();
    private _reconnecting: boolean = false;
    public connectionState: HubConnectionState = HubConnectionState.DISCONNECTED;
    public draftState: IDraftState;
    public draftConfig: IDraftConfigDTO;
    public connectedUsers: IDraftUser[] = [];

    constructor() {
      
        try {
            this.hub = $.connection['draftHub'];
            this.hub.client.updateConfig = (config: IDraftConfigDTO) => {
                this.draftConfig = config;
                this.draftState = config.state;
                this._draftConfigSubject.next(config);
            };

            this.hub.client.updateDraftState = (state: IDraftState) => {
                this.draftState = state;
                this._draftStateSubject.next(state);
            };

            this.hub.client.SetConnectedUsers = (users: IDraftUser[]) => {
                this.connectedUsers = users;
                this.sortUsers();
                this._connectedUsersSubject.next(users);
            };

            this.hub.client.OnUserJoined = (user: IDraftUser) => {
                let match = this.connectedUsers.find((item) => {
                    return item.id === user.id;
                });
                if (match) {
                    Object.assign(match, user);
                } else {
                    this.connectedUsers.push(user);
                }
                this.sortUsers();
                this._connectedUsersSubject.next(this.connectedUsers);
            };

            this.hub.client.OnUserStatusUpdate = (user: IDraftUser) => {
                let match = this.connectedUsers.find((item) => {
                    return item.id === user.id;
                });
                if (match) {
                    Object.assign(match, user);
                } else {
                    this.connectedUsers.push(user);
                }
                this.sortUsers();
                this._connectedUsersSubject.next(this.connectedUsers);
            };

            this.hub.client.OnUserLeft = (user: IDraftUser) => {
                let match = this.connectedUsers.find((item) => {
                    return item.id === user.id;
                });
                if (match) {
                    let idx = this.connectedUsers.indexOf(match);
                    this.connectedUsers.splice(idx, 1);
                    this.sortUsers();
                    this._connectedUsersSubject.next(this.connectedUsers);
                }
            };


            this.hub.connection.stateChanged((state: SignalR.StateChanged) => {
                switch (state.newState) {
                    case 0:
                        this.connectionState = HubConnectionState.CONNECTING;
                        break;
                    case 1:
                        this.connectionState = HubConnectionState.CONNECTED;
                        break;
                    case 2:
                        this.connectionState = HubConnectionState.RECONNECTING;
                        this.connectPromise = null;
                        break;
                    case 4:
                        this.connectionState = HubConnectionState.DISCONNECTED;
                        this.connectPromise = null;
                        break;
                }
                if (state.oldState === 2 && state.newState === 1) {
                    this.reconnect();
                } else if (state.newState === 4) {
                    this.reconnect(true);
                }
                this._connectionSubject.next(this.connectionState);
            });
        } catch (e) {
            console.error(e);
        }
        
    }

    private sortUsers() {
        this.connectedUsers.sort((a, b) => {
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

    public get draftConfigObservable(): Observable<IDraftConfigDTO> {
        return this._draftConfigSubject.asObservable();
    }

    public get connectionStateObservable(): Observable<HubConnectionState> {
        return this._connectionSubject.asObservable();
    }

    public get draftStateObservable(): Observable<IDraftState> {
        return this._draftStateSubject.asObservable();
    }

    public get connectedUserObsevable(): Observable<IDraftUser[]> {
        return this._connectedUsersSubject.asObservable();
    }

    private connect(): Promise<any> {
        if (!this.connectPromise) {
            this.connectPromise = new Promise<any>((resolve, reject) => {
                $.connection.hub.start().done(() => {
                    resolve();
                }).fail((err) => {
                    reject(err);
                });
            });
        }
        return this.connectPromise;
    }

    private reconnect(delayed = false): void {
        if (this.connectionArguments) {
            if (!this.connectPromise && !this._reconnecting) {
                this._reconnecting = true;
                if (delayed) {
                    console.log('Attemping Reconnect in 10s...');
                }
                setTimeout(() => {
                    console.log('Reconnecting...');
                    this.connect().then(() => {
                        this.connectToDraft(this.connectionArguments[0], this.connectionArguments[1]).then((config) => {
                            this.draftConfig = config;
                            this.draftState = config.state;
                            this._draftConfigSubject.next(config);
                            this._draftStateSubject.next(config.state);
                            this._reconnecting = false;
                            console.log('Reconnected.');
                        }).catch((err) => {
                            console.log('Reconnect fialed', err);
                            this._reconnecting = false;
                            this.connectPromise = null;
                            this.reconnect(true);
                        });
                    }, (err) => {
                        console.log('Reconnect fialed', err);
                        this._reconnecting = false;
                        this.connectPromise = null;
                        this.reconnect(true);
                    });
                }, delayed ? 10000 : 0);
            }
        }
    }

    public disconnect(): void {
        this.connectionArguments = null;
        this.connectPromise = null;
        this.hub.connection.stop(false, true);
    }

    public createDraft(createCfg: ICreateDraftDTO): Promise<IDraftConfigAdminDTO> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.createDraft(createCfg).then((config) => {
                    this.draftConfig = config;
                    this.draftState = config.state;
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public connectToDraft(draftToken: string, teamToken?: string): Promise<IDraftConfigDTO> {
        return new Promise((resolve, reject) => {
            this.connectionArguments = [draftToken, teamToken];
            this.connect().then(() => {
                this.hub.server.connectToDraft(draftToken, teamToken).then((config) => {
                    this.draftConfig = config;
                    this.draftState = config.state;
                    this._draftConfigSubject.next(config);
                    this._draftStateSubject.next(config.state);
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public resetDraft(draftToken: string, adminToken: string): Promise<IDraftConfigAdminDTO> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.restartDraft(draftToken, adminToken).then((config) => {
                    this.draftConfig = config;
                    this.draftState = config.state;
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public closeDraft(draftToken: string, adminToken: string): Promise<IDraftConfigAdminDTO> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.closeDraft(draftToken, adminToken).then(() => {
                    resolve();
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public setReady(draftToken: string, teamToken: string): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.setReady(draftToken, teamToken).then((success) => {
                    resolve(success);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public pickHero(heroId: string, draftToken: string, teamToken: string): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.pickHero(heroId, draftToken, teamToken).then((success) => {
                    resolve(success);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }
}
*/
