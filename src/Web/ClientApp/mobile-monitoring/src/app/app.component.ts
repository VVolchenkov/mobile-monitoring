import { Component, OnDestroy, OnInit } from '@angular/core';
import { IApiService } from '../interfaces/api-service';
import { Device } from '../models/device';
import { BehaviorSubject, filter, interval, of, skip, Subject, switchMap, takeUntil } from 'rxjs';
import { Event } from '../models/event';
import { HubService } from '../services/hub.service';

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
    getEvents$ = new BehaviorSubject<boolean>(false);
    getEvents = false;

    constructor(
        private readonly apiService: IApiService,
        private readonly hubService: HubService
    ) {}

    public ngOnInit(): void {
        this.apiService
            .getDevices()
            .pipe(takeUntil(this.componentDestroyed$))
            .subscribe((response) => {
                this.devices = response;
            });

        this.getEvents$
            .pipe(
                filter(x => x),
                switchMap(_ => interval(3000)
                    .pipe(
                        switchMap(_ =>
                            this.selectedDevice
                                ? this.apiService.getDeviceEvents(this.selectedDevice.id)
                                : of({ events: [] })
                        ),
                        takeUntil(this.componentDestroyed$))
                    )
                )
            .subscribe((response) => {
                this.events = response.events;
            });

        this.selectedDeviceId$
            .pipe(
                switchMap((deviceId) =>
                    deviceId
                        ? this.apiService.getDeviceEvents(deviceId)
                        : of({ events: [] })
                ),
                takeUntil(this.componentDestroyed$)
            )
            .subscribe((response) => {
                this.events = response.events;
            });

        this.hubService.connection.on('uploadDevice', (device) => {
            this.devices.push(new Device(device.id, device.name, device.os, device.version, device.lastUpdate));
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

    public setGetEvents(): void {
        this.getEvents = !this.getEvents;
        this.getEvents$.next(this.getEvents);
    }
}
