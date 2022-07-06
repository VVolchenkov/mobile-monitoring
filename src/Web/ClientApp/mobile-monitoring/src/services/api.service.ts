import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Device} from '../models/device';
import {IApiService} from '../interfaces/api-service';
import {Observable} from 'rxjs';
import {DeviceEvents} from '../models/device-events';
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class ApiService implements IApiService {
    constructor(private readonly http: HttpClient) {
    }

    public getDevices(): Observable<Device[]> {
        return this.http.get<Device[]>(`${environment.apiUrl}/api/devices`);
    }

    public getDeviceEvents(deviceId: string): Observable<DeviceEvents> {
        return this.http.get<DeviceEvents>(`${environment.apiUrl}/api/devices/${deviceId}/events`);
    }
}
