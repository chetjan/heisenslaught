import { Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';
import {
    SignalRHub, SignalRConnectionService, HubMethodHandler, SignalRConnectionState
    //  HubEventHandler, HubMethodHandler, SignalRConnectionState 
}
    from './/signalr-hub';


export interface IServerEventProxy {
    addListeners(contexts: string[]): Promise<void>;
    removeListeners(contexts: string[]): Promise<void>;
}

export interface ServerEvent<T> {
    context: string;
    type: string;
    data: T;
}

class ServerEventEmitter<T> extends Observable<ServerEvent<T>> {
    //public static listeningContexts: Set<string> = new Set();

    private subscriber: Subscriber<ServerEvent<T>>;
    constructor(sub: Function, unsub: Function) {
        super((subscriber: Subscriber<ServerEvent<T>>) => {
            this.subscriber = subscriber;
            sub();
            return () => {
                this.subscriber = undefined;
                unsub();
            };
        });
    }
    public emit(data: any): void {
        if (this.subscriber) {
            this.subscriber.next(data);
        }
    }
}

@Injectable()
export class ServerEventService extends SignalRHub<IServerEventProxy> {
    private emitters: Map<string, ServerEventEmitter<any>> = new Map();
    private eventObservables: Map<string, Observable<any>> = new Map();
    private subscribedContexts: Set<string> = new Set();
    private _reconnecting = false;
    constructor(signalRConnectionService: SignalRConnectionService) {
        super(signalRConnectionService, 'serverEventHub');
        this.connect();

    }

    @HubMethodHandler('emit')
    protected handleServerEvent(serverEvent: ServerEvent<any>) {
        if (this.emitters.has(serverEvent.context)) {
            let obs = this.emitters.get(serverEvent.context);
            obs.emit(serverEvent);
        }

    }

    public get(context: string): Observable<any> {
        if (!this.emitters.has(context)) {
            let emitter = new ServerEventEmitter(() => {
                if (!this.subscribedContexts.has(context)) {
                    this.subscribedContexts.add(context);
                    this.server.addListeners([context]);
                }
            }, () => {
                if (this.subscribedContexts.delete(context)) {
                    this.server.removeListeners([context]);
                }
            });
            this.emitters.set(context, emitter);
            this.eventObservables.set(context, emitter.share());
        }
        return this.eventObservables.get(context);
    }

    protected stateChange(newState: SignalRConnectionState, oldState: SignalRConnectionState) {
        super.stateChange(newState, oldState);
        if (newState !== oldState) {
            switch (newState) {
                case SignalRConnectionState.DISCONNECTED:
                    this._reconnecting = true;
                    /*setTimeout(() => {
                        this.reconnect();
                    }, 10000);
                    */
                    break;
                case SignalRConnectionState.RECONNECTING:
                    this._reconnecting = true;
                    break;
                case SignalRConnectionState.CONNECTED:
                    if (this._reconnecting) {
                        this.server.addListeners(Array.from(this.subscribedContexts));
                    }
                    this._reconnecting = false;
                    break;
            }
        }
    }

}
