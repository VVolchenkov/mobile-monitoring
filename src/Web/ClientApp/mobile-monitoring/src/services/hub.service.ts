import { Inject, Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { HubConnection } from '@aspnet/signalr';
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class HubService {
    public readonly connection: HubConnection;

    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${environment.apiUrl}/deviceHub`)
            .build();

        this.connection
            .start()
            .then((_) => console.log('signalR connection started'));
    }
}
