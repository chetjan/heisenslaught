import { Injectable } from '@angular/core';
import { Observable, Subscriber, Subscription } from 'rxjs';


export enum SignalRConnectionState {
    DISCONNECTED,  // 0
    CONNECTING, // 1
    INITALIZING, // 2
    CONNECTED, // 3
    RECONNECTING  // 4
}

/*
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
*/



export interface ISignalRConnection extends SignalRConnectionBase { }


export interface SignalRStateChange {
    newState: SignalRConnectionState;
    oldState: SignalRConnectionState;
    fromHandler?: boolean;
}

interface PartialObserver<T> {
    next?: (value: SignalRConnectionState) => void;
    error?: (error: any) => void;
    complete?: () => void;
}


interface IConnectable {
    state: number | SignalRConnectionState;
    start(): void;
    stop(): void;
}


abstract class SignalRConnectionBase {
    protected _connectionObserver: Observable<SignalRConnectionState>;
    protected _state: SignalRConnectionState;
    protected _stateChange: Observable<SignalRStateChange>;
    protected _stateChangeListener: any;
    protected _stateChangeSubscriber: Subscriber<SignalRStateChange>;
    protected _hasSubs: boolean = false;

    constructor() { }

    public get state(): SignalRConnectionState {
        return this._state;
    }

    public get stateChange(): Observable<SignalRStateChange> {
        return this._stateChange;
    }

    public get hasConnections(): boolean {
        return this._hasSubs;
    }

    public subscribe(): Subscription;
    public subscribe(observer: PartialObserver<SignalRConnectionState>): Subscription;
    public subscribe(next?: (value: SignalRConnectionState) => void, error?: (error: any) => void, complete?: () => void): Subscription;
    public subscribe(...args: any[]): Subscription {
        return this._connectionObserver.subscribe.apply(this._connectionObserver, args);
    }


    public abstract start(): void;
    public abstract stop(): void;


    protected abstract get connection(): IConnectable;
    //protected abstract initializeConnection(): void
    protected abstract initializeState(): void;
    protected abstract getState(srState: number): SignalRConnectionState;

    protected initialize(): void {
        this.initializeConnection();
        this.initializeState();
    }

    protected initializeConnection(): void {
        this._connectionObserver = Observable.create((subscriber: Subscriber<SignalRConnectionState>) => {
            this._hasSubs = true;
            console.log(this.constructor.name, 'on init', this.connection.state);
            /*this.handleStateChange({
                newState: this.connection.state,
                oldState: -1
            });*/
            let sub = this._stateChange.subscribe((state) => {
                subscriber.next(state.newState);
            });
            this.start();
            return () => {
                sub.unsubscribe();
                this._hasSubs = false;
                this.stop();
            };
        }).share();
    }
}


class SignalRConnection extends SignalRConnectionBase {
    private _url: string;
    private _connection: SignalR.Connection;
    private _started: boolean = false;
    constructor(url?: string) {
        super();
        this._url = url;
        this.initialize();
    }

    public get url(): string {
        return this.connection.url;
    }

    public start(): void {
        if (this._hasSubs && this._state === SignalRConnectionState.DISCONNECTED) {
            this._started = true;
            this.connection.start();
        }
    }

    public stop(notify = true): void {
        if (this._state !== SignalRConnectionState.DISCONNECTED) {
            this._started = false;
            this.connection.stop(false, notify);
        }
    }

    protected initializeState(): void {
        this._state = this.getState(this.connection.state);
        this._stateChange = Observable.create((subscriber: Subscriber<SignalRStateChange>) => {
            this._stateChangeSubscriber = subscriber;
            if (!this._stateChangeListener) {
                this._stateChangeListener = this.handleStateChange.bind(this);
                this.connection.stateChanged(this._stateChangeListener);
            }
            return () => {
                this._stateChangeSubscriber = undefined;
            };
        }).share();
    }

    protected get connection(): SignalR.Connection {
        if (!this._connection) {
            this._connection = $.connection.hub;
        }
        return this._connection;
    }

    protected getState(srState: number): SignalRConnectionState {
        let state: SignalRConnectionState;
        switch (srState) {
            case -1:
                state = SignalRConnectionState.INITALIZING;
                break;
            case 0:
                state = SignalRConnectionState.CONNECTING;
                break;
            case 1:
                state = SignalRConnectionState.CONNECTED;
                break;
            case 2:
                state = SignalRConnectionState.RECONNECTING;
                break;
            case 4:
                state = SignalRConnectionState.DISCONNECTED;
                break;
        }
        return state;
    }

    protected handleStateChange(state: SignalRStateChange) {
        let nState = this.getState(state.newState);
        let oState = this._state;
        this._state = nState;
        if (this._stateChangeSubscriber) {
            this._stateChangeSubscriber.next({
                newState: nState,
                oldState: oState
            });
        }
        if (this._state === SignalRConnectionState.DISCONNECTED && this._started) {
            console.log('UNEXPECTED DISCONNECT')
            setTimeout(() => {
                console.log('RECONNECTING')
                this.start();
            }, 10000);
        }
    }

}

class SignalRHubConnection extends SignalRConnectionBase {
    private _hubName: string;
    private _connection: SignalRConnection;
    private _connectionSubscription: Subscription;

    private _hubConnection: SignalR.Connection;


    constructor(hubName: string, connection: SignalRConnection) {
        super();
        this._hubName = hubName;
        this._hubConnection = $.connection[hubName].connection;
        this._connection = connection;
        this.initialize();
    }

    public get hubName(): string {
        return this._hubName;
    }


    public start(): void {
        if (this._hasSubs && this._state === SignalRConnectionState.DISCONNECTED) {
            if (!this._connectionSubscription) {
                this._connectionSubscription = this.connection.subscribe();
            }
        }
    }

    public stop(): void {
        if (this._state !== SignalRConnectionState.DISCONNECTED) {
            if (this._connectionSubscription) {
                this._connectionSubscription.unsubscribe();
            }
        }
    }

    protected initializeState(): void {
        this._state = this.getState(this.connection.state);
        this._stateChange = Observable.create((subscriber: Subscriber<SignalRStateChange>) => {
            this._stateChangeSubscriber = subscriber;
            let sub = this.connection.stateChange.subscribe((state) => {
                this._state = state.newState;
                subscriber.next(state);
            });
            return () => {
                sub.unsubscribe();
                this._stateChangeSubscriber = undefined;
            };
        }).share();
    }

    protected get connection(): SignalRConnection {
        return this._connection;
    }

    protected getState(srState: number): SignalRConnectionState {
        return srState;
    }

}


/*
class SignalRConnection extends Observable<SignalRConnectionState> implements ISignalRStateObservable {
    private _subscriber: Subscriber<SignalRConnectionState>;
    private _state: SignalRConnectionState = SignalRConnectionState.DISCONNECTED;
    private _hub: SignalR.Connection;
    private _stateChangeListener: any;

    public constructor(private _connectionName: string, private _isConnectionManager = false, shutdow: Function = null) {
        super((subscriber: Subscriber<SignalRConnectionState>) => {
            this._subscriber = subscriber;
            this._hub = $.hubConnection(); //$.connection[_hubName];

            if (!this._stateChangeListener) {
                this._stateChangeListener = this.handleStateChange.bind(this);
                this._hub.stateChanged(this._stateChangeListener);
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
        return this._connectionName;
    }

    public connect() {
        if (this._isConnectionManager) {

            if (this._subscriber !== null && this.state === SignalRConnectionState.DISCONNECTED) {
                this._hub.start();
            }
        }
    }

    public disconnect() {
        if (this._isConnectionManager) {
            if (this.state !== SignalRConnectionState.DISCONNECTED) {
                this._hub.stop();
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
*/
// TODO: All hubs share a connection
@Injectable()
export class SignalRConnectionService {
    private _connections: Map<string, SignalRConnection> = new Map();
    private _hubConnections: Map<string, Map<string, SignalRHubConnection>> = new Map();

    public getConnection(hubName: string, url?: string) {
        let urlKey = url === undefined ? '' : url;
        if (!this._connections.has(urlKey)) {
            this._connections.set(urlKey, new SignalRConnection(url));
        }

        if (!this._hubConnections.has(urlKey)) {
            this._hubConnections.set(urlKey, new Map());
        }
        let urlHubs = this._hubConnections.get(urlKey);

        if (!urlHubs.has(hubName)) {
            urlHubs.set(hubName, new SignalRHubConnection(hubName, this._connections.get(urlKey)));
        }

        return urlHubs.get(hubName);
    }

    public hasConnections(url?: string) {
        return this.getConnection(url).hasConnections;
    }

    public reconnectAll(notify = true): void {
        this._connections.forEach((connection) => {
            connection.stop(notify);
        });
        this._connections.forEach((connection) => {
            connection.start();
        });
    }

    public disconnectAll(notify = true): void {
        this._connections.forEach((connection) => {
            connection.stop(notify);
        });
    }

    public connectAll(): void {
        this._connections.forEach((connection) => {
            connection.start();
        });
    }

    /*    private _hubConnections: Map<string, ISignalRConnection> = new Map();
        private _hubStateObservers: Map<string, ISignalRStateObservable> = new Map();
    
        public getConnection(hubName: string): ISignalRConnection {
            hubName = 'serverEventHub';
            if (!this._hubConnections.has(hubName)) {
                let obs = this.createObservable(hubName, true);
                this._hubConnections.set(hubName, <ISignalRConnection>obs);
            }
            return this._hubConnections.get(hubName);
        }
    
        public getState(hubName: string): ISignalRStateObservable {
            hubName = 'serverEventHub';
            if (!this._hubStateObservers.has(hubName)) {
                let obs = this.createObservable(hubName, false);
                this._hubStateObservers.set(hubName, obs);
            }
            return this._hubStateObservers.get(hubName);
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
        */

}
