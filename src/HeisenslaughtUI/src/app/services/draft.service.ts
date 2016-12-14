import { Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs/Rx';

import { ICreateDraftResult, IDraftConfig, ICreateDraftData } from './draft-config';
import { IDraftState, DraftPhase } from './draft-state';
import { IDraftHubProxy } from './draft-hub-proxy';


export { ICreateDraftData, ICreateDraftResult, IDraftConfig } from './draft-config';
export { IDraftState, DraftPhase } from './draft-state';



/*import { Http, Response } from '@angular/http';

import { HeroData } from './hero';
*/




@Injectable()
export class DraftService {
    private hub: IDraftHubProxy;
    private connectPromise: Promise<any>;


    private _draftConfig: Observable<IDraftConfig>;
    private _draftConfigSub: Subscriber<IDraftConfig>;

    private _draftState: Observable<IDraftState>;
    private _draftStateSub: Subscriber<IDraftState>;


    constructor() {
        this.hub = $.connection['draftHub'];
        this.hub.client.updateConfig = (config: IDraftConfig) => {
            if (this._draftConfigSub) {
                this._draftConfigSub.next(config);
            }
        };

        this._draftConfig = new Observable<IDraftConfig>((sub: Subscriber<IDraftConfig>) => {
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
            console.log('msg ', args);
        };

        this.hub.client.getConnectedUsers = (...args: any[]) => {
            console.log('getConnectedUsers ', args);
        };
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

    public createDraft(createCfg: ICreateDraftData): Promise<ICreateDraftResult> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.configDraft(createCfg).then((config) => {
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public resetDraft(): Promise<ICreateDraftResult> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.resetDraft().then((config) => {
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public closeDraft(): Promise<ICreateDraftResult> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.closeDraft().then((config) => {
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public getCurrentAdminConfig(): Promise<ICreateDraftResult> {
        return new Promise((resolve, reject) => {
            this.connect().then(() => {
                this.hub.server.getCurrentAdminConfig().then((config) => {
                    resolve(config);
                }, (err) => {
                    reject(err);
                });
            }, (err) => {
                reject(err);
            });
        });
    }

    public connectToDraft(draftToken: string, teamToken?: string): Promise<IDraftConfig> {

        return new Promise((resolve, reject) => {
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


    public getDraftConfig(draftToken: string): Observable<IDraftConfig> {
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