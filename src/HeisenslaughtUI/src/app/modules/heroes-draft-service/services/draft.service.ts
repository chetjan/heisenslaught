import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs/Rx';

import { IDraftConfigAdminDTO, IDraftConfigDTO, ICreateDraftDTO } from './types/draft-config';
import { IDraftState } from './types/draft-state';
import { IDraftHubProxy } from './types/draft-hub-proxy';
import { HubConnectionState } from './types/hub-connection-sate';

export { ICreateDraftDTO, IDraftConfigAdminDTO, IDraftConfigDTO, IDraftConfigDrafterDTO } from './types/draft-config';
export { IDraftState, DraftPhase } from './types/draft-state';
export { HubConnectionState } from './types/hub-connection-sate';

@Injectable()
export class DraftService {
    private hub: IDraftHubProxy;
    private connectPromise: Promise<any>;
    private connectionArguments: string[];
    private _connectionSubject: Subject<HubConnectionState> = new Subject<HubConnectionState>();
    private _draftConfigSubject: Subject<IDraftConfigDTO> = new Subject<IDraftConfigDTO>();
    private _draftStateSubject: Subject<IDraftState> = new Subject<IDraftState>();
    private _reconnecting: boolean = false;
    public connectionState: HubConnectionState = HubConnectionState.DISCONNECTED;
    public draftState: IDraftState;
    public draftConfig: IDraftConfigDTO;

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

    public get draftConfigObservable(): Observable<IDraftConfigDTO> {
        return this._draftConfigSubject.asObservable();
    }

    public get connectionStateObservable(): Observable<HubConnectionState> {
        return this._connectionSubject.asObservable();
    }

    public get draftStateObservable(): Observable<IDraftState> {
        return this._draftStateSubject.asObservable();
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
