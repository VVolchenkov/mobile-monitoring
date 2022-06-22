import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Device } from '../models/device';
import { IApiService } from '../interfaces/api-service';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class ApiService implements IApiService {
    constructor(private readonly http: HttpClient) {}

    public getDevices(): Observable<Device[]> {
        return this.http.get<Device[]>('api/devices');
    }
}
