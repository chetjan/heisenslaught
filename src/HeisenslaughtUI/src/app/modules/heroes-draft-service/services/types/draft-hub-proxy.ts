import { ICreateDraftDTO, IDraftConfigAdminDTO, IDraftConfigDTO } from './draft-config';


export interface IDraftHubServerProxy {
    createDraft(config: ICreateDraftDTO): Promise<IDraftConfigAdminDTO>;
    restartDraft(draftToken: string, adminToken: string): Promise<IDraftConfigAdminDTO>;
    closeDraft(draftToken: string, adminToken: string): Promise<void>;
    connectToDraft(draftToken: string, teamToken?: string): Promise<IDraftConfigDTO>;
    setReady(draftToken: string, teamToken: string): Promise<boolean>;
    pickHero(heroId: string, draftToken: string, teamToken: string): Promise<boolean>;
}
