import { Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs/Rx';

import { ICreateDraftResult, IDraftConfig } from './draft-config';
import { IDraftState, DraftStatus } from './draft-status';

export { ICreateDraftResult, IDraftConfig } from './draft-config';
export { IDraftState, DraftStatus } from './draft-status';



/*import { Http, Response } from '@angular/http';

import { HeroData } from './hero';
*/




@Injectable()
export class DraftService {
    private _draftConfig: Observable<IDraftConfig>;
    private _draftStatus: Observable<IDraftState>;

    public createDraft(createCfg: IDraftConfig): Observable<ICreateDraftResult> {
        return null;
    }

    public getDraftConfig(draftToken: string): Observable<IDraftConfig> {
        if (!this._draftConfig) {
            this._draftConfig = new Observable<IDraftConfig>((sub: Subscriber<IDraftConfig>) => {
                setTimeout(() => {
                    sub.next({
                        team1Name: 'OnSlaught',
                        team2Name: 'OffSlaught',
                        firstPick: 2,
                        pickTime: 60,
                        bonusTime: 100,
                        bankTime: true,
                        mapName: 'Blackheart\'s Bay',
                        disabledHeroes: ['rexxar']
                    });
                }, 50);
            });
        }

        return this._draftConfig;
    }

    public getDraftState(draftToken: string, teamToken?: string): Observable<IDraftState> {
        if (!this._draftStatus) {
            this._draftStatus = new Observable<IDraftState>((sub: Subscriber<IDraftState>) => {
                let draftState: IDraftState = {
                    status: DraftStatus.WAITING,
                    pickTime: 60,
                    team1BonusTime: 100,
                    team2BonusTime: 100,
                    team1Ready: false,
                    team2Ready: false,
                    picks: []
                };

                let count = 0;

                setInterval(() => {
                    if (count === 4) {
                        draftState.team2Ready = true;
                    }
                    if (count === 7) {
                        draftState.team1Ready = true;
                    }

                    if (count === 15) {
                        draftState.status = DraftStatus.BANNING;
                    }

                    switch (count) {
                        case 20:
                            draftState.picks.push('abathur');
                            break;
                        case 25:
                            draftState.picks.push('brightwing');
                            break;
                        case 30:
                            draftState.picks.push('muradin');
                            break;
                        case 35:
                            draftState.picks.push('chen');
                            break;
                        case 40:
                            draftState.picks.push('jaina');
                            break;
                        case 45:
                            draftState.picks.push('uther');
                            break;
                        case 50:
                            draftState.picks.push('valla');
                            break;
                        case 55:
                            draftState.picks.push('varian');
                            break;
                        case 60:
                            draftState.picks.push('liming');
                            break;
                        case 65:
                            draftState.picks.push('murky');
                            break;
                        case 70:
                            draftState.picks.push('tlv');
                            break;
                        case 75:
                            draftState.picks.push('thrall');
                            break;
                        case 80:
                            draftState.picks.push('zarya');
                            break;
                        case 85:
                            draftState.picks.push('diablo');
                            break;
                    }

                    draftState.pickTime = 60 - Math.floor(count / 2);
                    draftState.pickTime = Math.max(draftState.pickTime, 0);

                    sub.next(draftState);
                    ++count;
                }, 500);
            });
        }

        return this._draftStatus;
    }

    public pick(heroId: string, team: number, draftToken: string, teamToken?: string): Observable<boolean> {
        return null;
    }

}