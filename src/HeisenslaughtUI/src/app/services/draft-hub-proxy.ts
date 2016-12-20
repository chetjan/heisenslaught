import { ICreateDraftDTO, IDraftConfigAdminDTO, IDraftConfigDTO } from './draft-config';


export interface IDraftHubServerProxy {
    createDraft(config: ICreateDraftDTO): JQueryPromise<IDraftConfigAdminDTO>;
    resetDraft(draftToken: string, adminToken: string): JQueryPromise<IDraftConfigAdminDTO>;
    closeDraft(draftToken: string, adminToken: string): JQueryPromise<IDraftConfigAdminDTO>;
    connectToDraft(draftToken: string, teamToken?: string): JQueryPromise<IDraftConfigDTO>;
    setReady(draftToken: string, teamToken: string): JQueryPromise<boolean>;
    pickHero(heroId: string, draftToken: string, teamToken: string): JQueryPromise<boolean>;
}

export interface IDraftHubProxy extends SignalR.Hub.Proxy {
    client: any;
    server: IDraftHubServerProxy;
}
