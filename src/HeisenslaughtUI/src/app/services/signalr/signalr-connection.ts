import { Injectable, ValueProvider, Optional } from '@angular/core';
import { Observable, Subscriber, Subscription, Observer } from 'rxjs';


export enum SignalRConnectionState {
    DISCONNECTED,
    CONNECTED,
    CONNECTING,
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

@Injectable()
export class SignalRConnection {
    private _connection: Observable<any>;
    private _connectionState: SignalRConnectionState = SignalRConnectionState.DISCONNECTED;
    private _singalR: SignalR.Hub.Connection;
    private _subscriber: Subscriber<SignalRConnectionState>;


    public static provideConfig(config: SignalRConnectionConfig): ValueProvider {
        return {
            provide: SignalRConnectionConfigDefinition,
            useValue: new SignalRConnectionConfigDefinition(config)
        };
    }


    constructor( @Optional() config: SignalRConnectionConfigDefinition) {
        this._singalR = $.hubConnection();
        this.configure(config);
        this._singalR.stateChanged((state) => {
            if (this._subscriber) {
                this.handleStateChange(state);
            }
        });
    }

    public get state(): SignalRConnectionState {
        return this._connectionState;
    }

    public subscribe(): Subscription;
    public subscribe(observer: Observer<SignalRConnectionState>): Subscription;
    public subscribe(next?: (value: SignalRConnectionState) => void, error?: (error: any) => void, complete?: () => void): Subscription;
    public subscribe(...args: any[]): Subscription {
        return this.connection.subscribe.apply(this._connection, args);
    }

    public reconnect() {
        if (this.state !== SignalRConnectionState.DISCONNECTED) {
            this._singalR.stop();
        }
        setTimeout(() => {
            this._singalR.start();
        }, 100);
    }

    private configure(config: SignalRConnectionConfigDefinition) {
        if (!config) {
            config = new SignalRConnectionConfigDefinition();
        }
        let def = config.definition;
        this._singalR.reconnectDelay = def.reconnectDelay;
        this._singalR.reconnectWindow = def.reconnectWindow;
        this._singalR.url = def.url;
        this._singalR.logging = def.logging;
    }

    private handleStateChange(state: SignalR.StateChanged) {
        switch (state.newState) {
            case 0:
                this._connectionState = SignalRConnectionState.CONNECTING;
                break;
            case 1:
                this._connectionState = SignalRConnectionState.CONNECTED;
                break;
            case 2:
                this._connectionState = SignalRConnectionState.RECONNECTING;
                break;
            case 4:
                this._connectionState = SignalRConnectionState.DISCONNECTED;
                break;
        }
        this._subscriber.next(this._connectionState);
    }

    private get connection(): Observable<SignalRConnectionState> {
        if (!this._connection) {
            this._connection = Observable.create((subscriber: Subscriber<SignalRConnectionState>) => {
                console.log('sub');
                this._subscriber = subscriber;
                this._singalR.start();
                return () => {
                    console.log('no subs');
                    this._subscriber = null;
                    this._singalR.stop();
                    this._connection = null;
                    this._connectionState = SignalRConnectionState.DISCONNECTED;
                };
            }).share();
        }
        return this._connection;
    }
}
