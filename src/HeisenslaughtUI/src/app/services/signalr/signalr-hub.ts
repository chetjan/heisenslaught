
interface HubPoxy extends SignalR.Hub.Proxy {
    server: {};
    client: {};
}


export class SignalRHub {

    private hub: HubPoxy;

    public constructor(hubName: string, autoConnect: boolean) {
        this.hub = $.connection[hubName];
        //$.connection.hub.
    }

}