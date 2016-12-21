import { IDraftState } from './draft-state';


export interface ICreateDraftDTO {
    firstPick: number;
    pickTime: number;
    bonusTime: number;
    bankTime: boolean;
    team1Name: string;
    team2Name: string;
    map: string;
    disabledHeroes?: string[];
}

export interface IDraftConfigDTO extends ICreateDraftDTO {
    state: IDraftState;
}

export interface IDraftConfigDrafterDTO extends IDraftConfigDTO {
    team: number;
}

export interface IDraftConfigAdminDTO extends IDraftConfigDTO {
    wasFirstPickRandom: boolean;
    draftToken: string;
    adminToken: string;
    team1DrafterToken: string;
    team2DrafterToken: string;
}
