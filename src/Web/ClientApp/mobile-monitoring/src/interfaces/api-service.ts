import {Device} from '../models/device';
import {Observable} from 'rxjs';

export abstract class IApiService {
    public abstract getDevices(): Observable<Device[]>;
}
