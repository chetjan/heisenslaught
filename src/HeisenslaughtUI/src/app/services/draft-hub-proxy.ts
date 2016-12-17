import { ICreateDraftDTO, IDraftConfigAdminDTO, IDraftConfigDTO } from './draft-config';


export interface IDraftHubServerProxy {
    createDraft(config: ICreateDraftDTO): JQueryPromise<IDraftConfigAdminDTO>;

    configDraft(config: ICreateDraftDTO): JQueryPromise<IDraftConfigAdminDTO>;
    getCurrentAdminConfig(): JQueryPromise<IDraftConfigAdminDTO>;
    resetDraft(): JQueryPromise<IDraftConfigAdminDTO>;
    closeDraft(): JQueryPromise<IDraftConfigAdminDTO>;
    connectToDraft(draftToken: string, teamToken?: string): JQueryPromise<IDraftConfigDTO>;
    setReady(draftToken: string, teamToken: string): JQueryPromise<boolean>;
    pickHero(heroId: string, draftToken: string, teamToken: string): JQueryPromise<boolean>;
}

export interface IDraftHubProxy extends SignalR.Hub.Proxy {
    client: any;
    server: IDraftHubServerProxy;
}
