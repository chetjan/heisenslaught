
export interface IDraftHubServerProxy {
    configDraft(config): JQueryPromise<any>;
}

export interface IDraftHubProxy extends SignalR.Hub.Proxy {
    client: any;
    server: IDraftHubServerProxy;
}
