export interface IDraftConfig {
    firstPick: number;
    pickTime: number;
    bonusTime: number;
    bankTime: boolean;
    team1Name: string;
    team2Name: string;
    mapName: string;
    disabledHeroes?: string[];
}



export interface ICreateDraftResult extends IDraftConfig {
    draftToken: string;
    team1DrafterToken: string;
    team2DrafterToken: string;
}
