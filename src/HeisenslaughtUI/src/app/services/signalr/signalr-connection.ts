import { Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';


export enum SignalRConnectionState {
    DISCONNECTED,
    CONNECTING,
    INITALIZING,
    CONNECTED,
    RECONNECTING
}

export interface SignalRConnectionConfig {
    reconnectDelay?: number;
    reconnectWindow?: number;
    logging?: boolean;
    url?: string;
}

const DefaultSignalRConnectionConfig: SignalRConnectionConfig = {
    reconnectDelay: $.connection.hub.reconnectDelay,
    reconnectWindow: $.connection.hub.reconnectWindow,
    url: $.connection.hub.url,
    logging: false
};

class SignalRConnectionConfigDefinition {
    public definition: SignalRConnectionConfig;

    constructor(config: SignalRConnectionConfig = null) {
        let cfg = config || {};
        this.definition = Object.assign({}, DefaultSignalRConnectionConfig, cfg);
    }
}

export interface ISignalRStateObservable extends Observable<SignalRConnectionState> {
    readonly state: SignalRConnectionState;
    readonly hubName: string;
}
export interface ISignalRConnection extends SignalRConnection { }

class SignalRConnection extends Observable<SignalRConnectionState> implements ISignalRStateObservable {
    private _subscriber: Subscriber<SignalRConnectionState>;
    private _state: SignalRConnectionState = SignalRConnectionState.DISCONNECTED;
    private _hub: SignalR.Hub.Proxy;
    private _stateChangeListener: any;

    public constructor(private _hubName: string, private _isConnectionManager = false, shutdow: Function = null) {
        super((subscriber: Subscriber<SignalRConnectionState>) => {
            this._subscriber = subscriber;
            this._hub = $.connection[_hubName];

            if (!this._stateChangeListener) {
                this._stateChangeListener = this.handleStateChange.bind(this);
                this._hub.connection.stateChanged(this._stateChangeListener);
            }
            this.handleStateChange({
                newState: this._hub.state,
                oldState: -1
            });

            this.connect();

            return () => {
                this.disconnect();
                this._subscriber = null;
                if (shutdow) {
                    shutdow();
                }
            };
        });
    }

    public get state(): SignalRConnectionState {
        return this._state;
    }

    public get hubName() {
        return this._hubName;
    }

    public connect() {
        if (this._isConnectionManager) {

            if (this._subscriber !== null && this.state === SignalRConnectionState.DISCONNECTED) {
                this._hub.connection.start();
            }
        }
    }

    public disconnect() {
        if (this._isConnectionManager) {
            if (this.state !== SignalRConnectionState.DISCONNECTED) {
                this._hub.connection.stop();
            }
        }
    }

    private handleStateChange(state: SignalR.StateChanged): void {
        switch (state.newState) {
            case 0:
                this._state = SignalRConnectionState.CONNECTING;
                break;
            case 1:
                this._state = SignalRConnectionState.CONNECTED;
                break;
            case 2:
                this._state = SignalRConnectionState.RECONNECTING;
                break;
            case 4:
                this._state = SignalRConnectionState.DISCONNECTED;
                break;
        }
        if (this._subscriber) {
            this._subscriber.next(this._state);
        }
    }
}


@Injectable()
export class SignalRConnectionService {
    private _hubConnections: Map<string, ISignalRConnection> = new Map();
    private _hubStateObservers: Map<string, ISignalRStateObservable> = new Map();

    public getConnection(hubName: string): ISignalRConnection {
        if (!this._hubConnections.has(hubName)) {
            let obs = this.createObservable(hubName, true);
            this._hubConnections.set(hubName, <ISignalRConnection>obs);
        }
        return this._hubConnections.get(hubName);
    }

    public getState(hubName: string): ISignalRStateObservable {
        if (!this._hubStateObservers.has(hubName)) {
            let obs = this.createObservable(hubName, false);
            this._hubStateObservers.set(hubName, obs);
        }
        return this._hubStateObservers.get(hubName);
    }

    public reconnectAll(): void {
        this._hubConnections.forEach((connection) => {
            connection.disconnect();
        });
        this._hubConnections.forEach((connection) => {
            connection.connect();
        });
    }


    private createObservable(hubName: string, manager: boolean, shutdown: Function = null): ISignalRStateObservable {
        let obs = new SignalRConnection(hubName, manager, shutdown);
        let obsProxy = obs.share();
        Object.defineProperty(obsProxy, 'state', {
            get: () => {
                return obs.state;
            }
        });
        Object.defineProperty(obsProxy, 'hubName', {
            get: () => {
                return obs.hubName;
            }
        });
        if (manager) {
            obsProxy['connect'] = obs.connect.bind(obs);
            obsProxy['disconnect'] = obs.disconnect.bind(obs);
        }
        return <ISignalRStateObservable>obsProxy;
    }

}
