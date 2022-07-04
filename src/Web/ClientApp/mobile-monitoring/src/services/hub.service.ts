import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { HubConnection } from '@aspnet/signalr';

@Injectable({
    providedIn: 'root',
})
export class HubService {
    public readonly connection: HubConnection;

    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:7232/deviceHub')
            .build();

        this.connection.start().then((_) => console.log('signalR connection started'));
    }
}
