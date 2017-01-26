
import { Observable, Subscription, Subscriber } from 'rxjs';
import { ISignalRConnection, SignalRStateChange, SignalRConnectionState, SignalRConnectionService } from './signalr-connection';
export * from './signalr-connection';

// ulgly hack to allow semi lazy hubs
export function Hub(hubName: string): ClassDecorator {
    return (target: any): void => {
        Reflect.defineMetadata(HubMethodHandler, hubName, target);
        let hubKey = hubName.toLowerCase();
        let proxies: Object = $.connection.hub.proxies;
        if (proxies.hasOwnProperty(hubKey)) {
            let client = proxies[hubKey].client;

            let clientMethods: Array<{ name: string, method: Function, self?: any }> =
                Reflect.getMetadata(HubMethodHandler, target.prototype) || [];
            for (let i = 0; i < clientMethods.length; i++) {
                let def = clientMethods[i];
                client[def.name] = function (...args: any[]) {
                    return def.method.apply(def.self, args);
                };
            }

            let eventProperties: Array<{ name: string, property: string, callback?: Function }> =
                Reflect.getMetadata(HubEventHandler, target.prototype) || [];
            for (let i = 0; i < eventProperties.length; i++) {
                let def = eventProperties[i];
                client[def.name] = function (...args: any[]) {
                    def.callback.apply(def.callback, args);
                };
            }
        }
    };
}

export function HubMethodHandler(methodName: string = null): MethodDecorator {
    return (target: Object, propertyKey: string | symbol, descriptor: TypedPropertyDescriptor<any>): void => {
        let clientMethods = Reflect.getMetadata(HubMethodHandler, target) || [];
        clientMethods.push({
            name: methodName || propertyKey,
            method: descriptor.value
        });
        Reflect.defineMetadata(HubMethodHandler, clientMethods, target);
    };
}

export function HubEventHandler(methodName: string = null): PropertyDecorator {
    return (target: Object, propertyKey: string | symbol): void => {
        let eventProperties = Reflect.getMetadata(HubEventHandler, target) || [];
        eventProperties.push({
            property: propertyKey,
            name: methodName || propertyKey
        });
        Reflect.defineMetadata(HubEventHandler, eventProperties, target);
    };
}

interface HubPoxy<TServerHub> extends SignalR.Hub.Proxy {
    server: TServerHub;
    client: {};
}

export abstract class SignalRHub<TServerHub extends any> {
    private _connection: ISignalRConnection;
    private _stateObserver: Observable<SignalRStateChange>;
    private _state: SignalRConnectionState;
    private _serverMethods: TServerHub;
    private _hub: HubPoxy<TServerHub>;
    private _subscription: Subscription;
    private _hasEventSubs: boolean = false;
    private _hubName: string;

    public constructor(signalRConnectionService: SignalRConnectionService, hubName: string, connectionUrl?: string) {
        this._hubName = hubName;
        this._connection = signalRConnectionService.getConnection(hubName, connectionUrl);
        this._stateObserver = this._connection.stateChange;
        this._state = this._connection.state;
        this._hub = $.connection[hubName];
        this.setupHubEvents();
        this.setupClientMethods();
        this.setupServerMethods();
        this._stateObserver.subscribe((state) => {
            this.handleStateChanged(state);
        });
    }

    public get hubName(): string {
        return this._hubName;
    }

    public get server(): TServerHub {
        return this._serverMethods;
    }

    public get state(): SignalRConnectionState {
        return this._state;
    }

    public get stateObserver(): Observable<SignalRStateChange> {
        return this._stateObserver;
    }

    public get isSubscribed(): boolean {
        return !!this._subscription;
    }

    public get hasEventSubscriptions(): boolean {
        return this._hasEventSubs;
    }

    public get hasSubscriptions(): boolean {
        return this.hasEventSubscriptions || this.isSubscribed;
    }

    public connect() {
        if (!this._subscription) {
            this._subscription = this._connection.subscribe();
        }
    }

    public disconnect() {
        if (this._subscription) {
            this._subscription.unsubscribe();
            this._subscription = null;
        }
    }

    public reconnect() {
        if (this._state === SignalRConnectionState.DISCONNECTED) {
            if (this._hasEventSubs || this._subscription) {
                this._connection.start();
            }
        }
    }

    public emitClientEvent(name: string, event: any) {
        if (typeof (this._hub.client[name]) === 'function') {
            this._hub.client[name](event);
        }
    }

    private setupClientMethods(): void {
        let clientMethods: Array<{ name: string, method: Function, self?: any }> =
            Reflect.getMetadata(HubMethodHandler, this.constructor.prototype) || [];
        clientMethods.forEach(value => {
            value.self = this;
        });
    }

    private setupHubEvents(): void {
        let eventProperties: Array<{ name: string, property: string, callback?: Function }> =
            Reflect.getMetadata(HubEventHandler, this.constructor.prototype) || [];
        eventProperties.forEach(value => {
            let currSubscriber: Subscriber<SignalRConnectionState> = null;
            value.callback = (event: any) => {
                console.log('event callback', currSubscriber);
                if (currSubscriber) {
                    currSubscriber.next(event);
                }
            };
            this[value.property] = Observable.create((subscriber: Subscriber<SignalRConnectionState>) => {
                let sub = this._connection.subscribe();
                this._hasEventSubs = true;
                currSubscriber = subscriber;
                return () => {
                    this._hasEventSubs = false;
                    currSubscriber = null;
                    sub.unsubscribe();
                };
            }).share();
        });
    }

    private setupServerMethods(): void {
        this._serverMethods = <TServerHub>{};
        for (let methodName in this._hub.server) {
            if (this._hub.server.hasOwnProperty(methodName) && typeof (this._hub.server[methodName]) === 'function') {
                if (this._serverMethods[methodName]) {
                    throw new Error('Duplicate server method "' + methodName + '" on hub "' + this.hubName + '"');
                }
                this._serverMethods[methodName] = (...args: any[]): Promise<any> => {
                    return new Promise((resolve, reject) => {
                        let sub;
                        let callProxy = () => {
                            (<Function>this._hub.server[methodName]).apply(this._hub.server, args)
                                .done((result) => {
                                    resolve(result);
                                })
                                .fail((err) => {
                                    reject(err);
                                })
                                .always(() => {
                                    sub.unsubscribe();
                                });
                        };
                        if (this._connection.state === SignalRConnectionState.CONNECTED) {
                            sub = this._connection.subscribe();
                            callProxy();
                        } else {
                            sub = this._connection.subscribe((state) => {
                                if (state === SignalRConnectionState.CONNECTED) {
                                    callProxy();
                                }
                            });
                        }
                    });
                };
            }
        }
    }

    protected stateChange(newState: SignalRConnectionState, oldState: SignalRConnectionState): void { }

    private handleStateChanged(state: SignalRStateChange): void {
        this._state = state.newState;
        this.stateChange(state.newState, state.oldState);
    }
}


