import { ICreateDraftData, ICreateDraftResult, IDraftConfig } from './draft-config';

export interface IDraftHubServerProxy {
    configDraft(config: ICreateDraftData): JQueryPromise<ICreateDraftResult>;
    getCurrentAdminConfig(): JQueryPromise<ICreateDraftResult>;
    resetDraft(): JQueryPromise<ICreateDraftResult>;
    closeDraft(): JQueryPromise<ICreateDraftResult>;
    connectToDraft(draftToken: string, teamToken?: string): JQueryPromise<IDraftConfig>;
    setReady(draftToken: string, teamToken: string): JQueryPromise<boolean>;
    pickHero(heroId: string, draftToken: string, teamToken: string): JQueryPromise<boolean>;
}

export interface IDraftHubProxy extends SignalR.Hub.Proxy {
    client: any;
    server: IDraftHubServerProxy;
}
