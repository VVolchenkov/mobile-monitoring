import {Device} from '../models/device';
import {Observable} from 'rxjs';
import {DeviceEvents} from '../models/device-events';

export abstract class IApiService {
    public abstract getDevices(): Observable<Device[]>;

    public abstract getDeviceEvents(
        deviceId: string
    ): Observable<DeviceEvents>;
}
