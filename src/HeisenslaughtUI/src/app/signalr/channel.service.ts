import { Observable, Subject } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { ConnectionState } from './state';
import { ChannelSubject } from './subject';
import { ChannelConfig } from './config';
import { ChannelEvent } from './event';
import { MethodCall } from './method';
import 'jquery';
// import 'signalr';

export { ChannelConfig } from './config';


@Injectable()
export class ChannelService {
    /*private _start: Observable<any>;
    private _state: Observable<ConnectionState>;
    private _error: Observable<string>;

    private _hubConnection:  SignalR.Hub.Connection;
    private _hubProxy: any;

    private _subjects: ChannelSubject[];
    */

    private connectionState: Observable<ConnectionState>;
    private connectionStateSubject = new Subject<ConnectionState>();
    private errorObs: Observable<SignalR.ConnectionError>;
    private errorSubject = new Subject<SignalR.ConnectionError>();
    private startObs: Observable<any>;
    private startSubject = new Subject<any>();
    private hubConnection: SignalR.Hub.Connection;
    private hubProxy: SignalR.Hub.Proxy;

    private subjects = new Array<ChannelSubject>();
    private methods = new Array<MethodCall<any>>();

    constructor(
        @Inject('channel.config') private channelConfig: ChannelConfig
    ) {
        if ($ === undefined || $.hubConnection === undefined) {
            throw new Error('Signalr library not foundF');
        }

        //$.connection.hub.logging = true;
/*
        this.connectionState = this.connectionStateSubject.asObservable();
        this.errorObs = this.errorSubject.asObservable();
        this.startObs = this.startSubject.asObservable();

        this.hubConnection = $.hubConnection();
        this.hubConnection.url = channelConfig.url;
        this.hubProxy = this.hubConnection.createHubProxy(channelConfig.hub);

        this.hubConnection.received((msg) => {
            if (msg.E) {
                console.log('error', msg);
            } else {
                if (msg.H && msg.M) {
                    let methName = msg.M;
                    let meth: MethodCall<any> = this.methods.find((m: MethodCall<any>) => {
                        return m.methodName === methName;
                    });
                    if (meth !== undefined) {
                        meth.method.apply(undefined, msg.A || []);
                    }
                }
            }

            // let channel = msg.H .toLowerCase();
            
        });
        //this.hubConnection.

        this.hubConnection.stateChanged((state) => {
            console.log('state', state);
            let nState: ConnectionState;
            switch (state.newState) {
                case $.signalR.connectionState.connecting:
                    nState = ConnectionState.CONNECTING;
                    break;
                case $.signalR.connectionState.connected:
                    nState = ConnectionState.CONNECTED;
                    break;
                case $.signalR.connectionState.reconnecting:
                    nState = ConnectionState.RECONNECTING;
                    break;
                case $.signalR.connectionState.disconnected:
                    nState = ConnectionState.DISCONNECTED;
                    break;
            }
            this.connectionStateSubject.next(nState);
        });

        this.hubConnection.error((error) => {
            console.log('eee', error);
            this.errorSubject.next(error);
        });

        this.hubProxy.on('onEvent', (channel: string, event: ChannelEvent) => {
            let chanSub: ChannelSubject = this.subjects.find((subj: ChannelSubject) => {
                return subj.channel === channel;
            });
            if (chanSub !== undefined) {
                return chanSub.subject.next(event);
            }
        })
        ;
        */
    }

    public connect(): void {
        this.hubConnection.start().done(() => {
            this.startSubject.next();
            console.log('hubConnection', this.hubConnection);
           


        }).fail((error) => {
            this.startSubject.error(error);
        });
    }

    public getChannel(channel: string): Observable<ChannelEvent> {
        let channelSub: ChannelSubject = this.subjects.find((chan: ChannelSubject) => {
            return chan.channel === channel;
        });

        if (channelSub === undefined) {
            channelSub = new ChannelSubject();
            channelSub.channel = channel;
            channelSub.subject = new Subject<ChannelEvent>();
            this.subjects.push(channelSub);

            this.startObs.subscribe(() => {
                /* this.hubProxy.invoke('Subscribe', channel)
                     .done(() => {
                         console.log('Connected to ${channel}.');
                     })
                     .fail((error) => {
                         channelSub.subject.error(error);
                     });*/
            }, (error) => {

                channelSub.subject.error(error);
            });
        }
        return channelSub.subject.asObservable();
    }

    public registerMethod<T extends Function>(methodName: string, method: T): void {
        let meth: MethodCall<T> = this.methods.find((m: MethodCall<T>) => {
            return m.methodName === methodName;
        });
        if (meth === undefined) {
            meth = new MethodCall<T>();
            meth.methodName = methodName;
            meth.method = method;
            this.methods.push(meth);
        }
    }
}
