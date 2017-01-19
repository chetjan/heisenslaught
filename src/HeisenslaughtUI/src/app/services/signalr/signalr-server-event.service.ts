import { Injectable } from '@angular/core';
//import { Observable, Subject } from 'rxjs';
import {
    SignalRHub, SignalRConnectionService, HubMethodHandler
   //  HubEventHandler, HubMethodHandler, SignalRConnectionState 
}
    from './/signalr-hub';


export interface IServerEventProxy {
    addListeners(contexts: string[]): Promise<void>;
    removeListeners(contexts: string[]): Promise<void>;
}

@Injectable()
export class ServerEventService extends SignalRHub<IServerEventProxy> {
    constructor(signalRConnectionService: SignalRConnectionService) {
        super(signalRConnectionService, 'serverEventHub');
        this.connect();
/*
        setInterval(() => {
            this.server.addListeners(['system.broadcast.all', 'system.broadcast.users']);
        }, 5000);*/
    }

    @HubMethodHandler()
    private someCallback(){

    }

}
