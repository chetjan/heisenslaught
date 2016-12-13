import { IDraftState } from './draft-state';


export interface ICreateDraftData {
    firstPick: number;
    pickTime: number;
    bonusTime: number;
    bankTime: boolean;
    team1Name: string;
    team2Name: string;
    map: string;
    disabledHeroes?: string[];
}

export interface IDraftConfig extends ICreateDraftData {
    state: IDraftState;
}

export interface ICreateDraftResult extends IDraftConfig {
    randomFirstPick: boolean;
    draftToken: string;
    team1DrafterToken: string;
    team2DrafterToken: string;
}
