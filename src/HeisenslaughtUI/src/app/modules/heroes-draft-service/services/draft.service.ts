import { Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs/Rx';

import { IDraftConfigAdminDTO, IDraftConfigDTO, ICreateDraftDTO } from './types/draft-config';
import { IDraftState } from './types/draft-state';
import { IDraftHubProxy } from './types/draft-hub-proxy';
import { HubConnectionState } from './types/hub-connection-sate';

export { ICreateDraftDTO, IDraftConfigAdminDTO, IDraftConfigDTO, IDraftConfigDrafterDTO } from './types/draft-config';
export { IDraftState, DraftPhase } from './types/draft-state';


@Injectable()
export class DraftService{
    private hub: IDraftHubProxy;
    private connectPromise: Promise<any>;

    private _connectionSate: Observable<HubConnectionState>;
    private _connectionStateSub: Subscriber<HubConnectionState>;

    private _draftConfig: Observable<IDraftConfigDTO>;
    private _draftConfigSub: Subscriber<IDraftConfigDTO>;
    private _draftState: Observable<IDraftState>;
    private _draftStateSub: Subscriber<IDraftState>;

    private connectionArguments: string[];
    public connectionState: HubConnectionState = HubConnectionState.DISCONNECTED;

    constructor() {
        try {
            this.hub = $.connection['draftHub'];
            this.hub.client.updateConfig = (config: IDraftConfigDTO) => {
                if (this._draftConfigSub) {
                    this._draftConfigSub.next(config);
                }
            };

            this._draftConfig = new Observable<IDraftConfigDTO>((sub: Subscriber<IDraftConfigDTO>) => {
                this._draftConfigSub = sub;
            });

            this.hub.client.updateDraftState = (state: IDraftState) => {
                if (this._draftStateSub) {
                    this._draftStateSub.next(state);
                }
            };

            this._draftState = new Observable<IDraftState>((sub: Subscriber<IDraftState>) => {
                this._draftStateSub = sub;
            });

            this.hub.client.messageReceived = (...args: any[]) => {
                //   console.log('msg ', args);
            };

            this.hub.client.getConnectedUsers = (...args: any[]) => {
                //  console.log('getConnectedUsers ', args);
            };
            this.hub.connection.connectionSlow(() => {
                console.log('connection slow.....');
            });

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
            });

            this._connectionSate = new Observable<HubConnectionState>((sub: Subscriber<HubConnectionState>) => {
                this._connectionStateSub = sub;
            });
        } catch (e) {
            console.error(e);
        }
    }

    private reconnect(delayed = false): void {
        if (this.connectionArguments) {
            console.log('reconnecting...', this.connectionArguments);
            setTimeout(() => {
                this.connectToDraft(this.connectionArguments[0], this.connectionArguments[1]).then((config) => {
                    if (this._draftConfigSub) {
                        this._draftConfigSub.next(config);
                    }
                    if (this._draftStateSub) {
                        this._draftStateSub.next(config.state);
                    }
                    console.log('reconnected.');
                }).catch((err) => {
                    console.log('error reconnecting', err);
                    this.reconnect(true);
                });
            }, delayed ? 5000 : 0);
        }
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

    public disconnect(): void {
       this.connectionArguments = null;
       this.connectPromise = null;
       this.hub.connection.stop(false, true);
    }

    public createDraft(createCfg: ICreateDraftDTO): Promise<IDraftConfigAdminDTO> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.createDraft(createCfg).then((config) => {
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

    public connectToDraft(draftToken: string, teamToken?: string): Promise<IDraftConfigDTO> {
        return new Promise((resolve, reject) => {
            this.connectionArguments = [draftToken, teamToken];
            this.connect().then(() => {
                this.hub.server.connectToDraft(draftToken, teamToken).then((config) => {
                    resolve(config);
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

    public getDraftConfig(draftToken: string): Observable<IDraftConfigDTO> {
        return this._draftConfig;
    }

    public getDraftState(draftToken: string): Observable<IDraftState> {
        return this._draftState;
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
