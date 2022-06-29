import {Component, OnDestroy, OnInit} from '@angular/core';
import {IApiService} from '../interfaces/api-service';
import {Device} from '../models/device';
import {DeviceEvents} from '../models/device-events';
import {BehaviorSubject, of, skip, Subject, switchMap, takeUntil} from 'rxjs';
import {Event} from '../models/event';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
    componentDestroyed$: Subject<boolean> = new Subject();
    devices: Device[] = [];
    events: Event[] = [];
    selectedDevice: Device | undefined;
    selectedDeviceId$ = new BehaviorSubject<string>('');

    constructor(private readonly apiService: IApiService) {
    }

    public ngOnInit(): void {
        this.apiService
            .getDevices()
            .pipe(takeUntil(this.componentDestroyed$))
            .subscribe((response) => {
                this.devices = response;
            });

        this.selectedDeviceId$
            .pipe(
                switchMap((deviceId) =>
                    deviceId
                        ? this.apiService.getDeviceEvents(deviceId)
                        : of({events: []})
                ),
                takeUntil(this.componentDestroyed$)
            )
            .subscribe((response) => {
                this.events = response.events;
            });
    }

    public ngOnDestroy(): void {
        this.componentDestroyed$.next(true);
        this.componentDestroyed$.complete();
    }

    public setSelectedDeviceId(deviceId: string): void {
        const selectedDevice = this.devices.find((x) => x.id === deviceId);

        if (selectedDevice) {
            this.selectedDevice = selectedDevice;
            this.selectedDeviceId$.next(selectedDevice.id);
        }
    }
}
