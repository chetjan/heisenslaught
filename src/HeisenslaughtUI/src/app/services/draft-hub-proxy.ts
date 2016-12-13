import { ICreateDraftData, ICreateDraftResult } from './draft-config';

export interface IDraftHubServerProxy {
    configDraft(config: ICreateDraftData): JQueryPromise<ICreateDraftResult>;
    getCurrentAdminConfig(): JQueryPromise<ICreateDraftResult>;
    resetDraft(): JQueryPromise<ICreateDraftResult>;
    closeDraft(): JQueryPromise<ICreateDraftResult>;
}

export interface IDraftHubProxy extends SignalR.Hub.Proxy {
    client: any;
    server: IDraftHubServerProxy;
}
