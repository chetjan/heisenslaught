
import { Observable, Subscription, Subscriber } from 'rxjs';
import { ISignalRConnection, ISignalRStateObservable, SignalRConnectionState, SignalRConnectionService } from './signalr-connection';
export * from './signalr-connection';
// for test
import { Injectable } from '@angular/core';


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

export abstract class SignalRHub<TServerHub> {
    private _connection: ISignalRConnection;
    private _stateObserver: ISignalRStateObservable;
    private _state: SignalRConnectionState;
    private _serverMethods: TServerHub;
    private _hub: HubPoxy<TServerHub>;
    private _subscription: Subscription;
    private _hasEventSubs: boolean = false;

    public constructor(signalRConnectionService: SignalRConnectionService, hubName: string) {
        this._connection = signalRConnectionService.getConnection('publicServerEventHub');
        this._stateObserver = signalRConnectionService.getState('publicServerEventHub');
        this._state = this._stateObserver.state;
        this._hub = $.connection[hubName];
        this.setupHubEvents();
        this.setupClientMethods();
        this.setupServerMethods();
        this._stateObserver.subscribe((state) => {
            this.handleStateChanged(state);
        });
    }

    public get hubName(): string {
        return this._connection.hubName;
    }

    public get server(): TServerHub {
        return this._serverMethods;
    }

    public get state(): SignalRConnectionState {
        return this._state;
    }

    public get stateObserver(): ISignalRStateObservable {
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
                this._connection.connect();
            }
        }
    }

    public emitClientEvent(name: string, event: any) {
        if (typeof (this._hub.client[name]) === 'function') {
            this._hub.client[name](event);
        }
    }

    private setupClientMethods(): void {
        let clientMethods: Array<{ name: string, method: Function }> =
            Reflect.getMetadata(HubMethodHandler, this.constructor.prototype) || [];
        clientMethods.forEach(value => {
            if (this._hub.client[value.name]) {
                throw new Error('Duplicate client method "' + value.name + '" on hub "' + this.hubName + '"');
            }
            this._hub.client[value.name] = value.method.bind(this);
        });
    }

    private setupHubEvents(): void {
        let eventProperties: Array<{ name: string, property: string }> =
            Reflect.getMetadata(HubEventHandler, this.constructor.prototype) || [];
        eventProperties.forEach(value => {
            let currSubscriber: Subscriber<SignalRConnectionState> = null;
            if (this._hub.client[value.name]) {
                throw new Error('Duplicate event method "' + value.name + '" on hub "' + this.hubName + '"');
            }
            this._hub.client[value.name] = (event: any) => {
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
                this._serverMethods[methodName] = (...args: any[]) => {
                    return new Promise((resolve, reject) => {
                        let sub;
                        let callProxy = () => {
                            this._hub.server[methodName].apply(this._hub.server, args)
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
                                console.log(this._connection.state);
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

    private handleStateChanged(state: SignalRConnectionState): void {
        let oState = this._state;
        this._state = state;
        this.stateChange(state, oState);
    }
}



// Test Stuff

interface MyServerHub {
    addListeners(): Promise<void>;
}

@Injectable()
export class SignalRTestService extends SignalRHub<MyServerHub> {

    @HubEventHandler('abc')
    public onSomething: Observable<any>;

    @HubEventHandler()
    public onSomethingElse: Observable<any>;

    constructor(signalRConnectionService: SignalRConnectionService) {
        super(signalRConnectionService, 'publicServerEventHub');
/*
        //this.server.addListeners();
        let sub = this.onSomething.subscribe(() => {
            // do stuff here 
        });
        let sub2: Subscription;

        setTimeout(() => {
            sub.unsubscribe();
        }, 5000);

        setTimeout(() => {
            sub = this.onSomething.subscribe(() => {
                // do stuff here 
            });
        }, 8000);
        setTimeout(() => {
            sub2 = this.onSomethingElse.subscribe(() => {
                // do stuff here 
            });
        }, 9000);

        setTimeout(() => {
            sub.unsubscribe();
        }, 10000);

        setTimeout(() => {
            sub2.unsubscribe();
        }, 20000);
*/
    }

    @HubMethodHandler()
    public updateDraftState(): void {
        console.log('updated draft state');
    }


    protected stateChange(newState: SignalRConnectionState, oldState: SignalRConnectionState): void {
        super.stateChange(newState, oldState);
        console.log('State changed from ' + SignalRConnectionState[oldState] + ' to ' + SignalRConnectionState[newState]);
    }
}
