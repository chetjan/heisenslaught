export enum DraftStatus {
    WAITING,
    PICKING,
    BANNING,
    FINISHED
}

export interface IDraftState {
    team1Ready: boolean;
    team2Ready: boolean;
    pickTime: number;
    team1BonusTime: number;
    team2BonusTime: number;
    picks: string[];
    status: DraftStatus;
}


