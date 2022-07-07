import { Device } from '../models/device';
import { Observable } from 'rxjs';
import { DeviceEvents } from '../models/device-events';
import { Event } from '../models/event';

export abstract class IApiService {
    public abstract getDevices(): Observable<Device[]>;

    public abstract getDeviceEvents(deviceId: string): Observable<DeviceEvents>;

    public abstract deleteDeviceEvents(deviceId: string): Observable<Object>;

    public abstract updateDeviceEvent(deviceId: string, event: Event): Observable<Object>
}
