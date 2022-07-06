import {Component, OnDestroy, OnInit} from '@angular/core';
import {IApiService} from '../interfaces/api-service';
import {Device} from '../models/device';
import {
    BehaviorSubject,
    combineLatest,
    interval,
    map, Observable,
    of,
    Subject,
    switchMap,
    takeUntil
} from 'rxjs';
import {Event} from '../models/event';
import {HubService} from '../services/hub.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
    componentDestroyed$: Subject<boolean> = new Subject();
    devices: Device[] = [];
    selectedDevice: Device | undefined;
    selectedDeviceId$ = new BehaviorSubject<string>('');
    getEvents$ = new BehaviorSubject<boolean>(false);
    events$: Observable<Event[] | any> = new Observable<Event[] | any>();
    automaticallyLoadEvents = false;

    constructor(
        private readonly apiService: IApiService,
        private readonly hubService: HubService
    ) {
    }

    public ngOnInit(): void {
        this.apiService
            .getDevices()
            .pipe(takeUntil(this.componentDestroyed$))
            .subscribe((response) => {
                this.devices = response;
            });

        this.events$ = combineLatest([this.getEvents$, this.selectedDeviceId$])
            .pipe(
                switchMap(([getEvents, id]) =>
                    getEvents ? interval(3000).pipe(map((_) => id)) : of(id)
                ),
                switchMap((x) =>
                    x ? this.apiService.getDeviceEvents(x) : of({events: []})
                ),
                map(x => x.events),
                takeUntil(this.componentDestroyed$)
            );

        this.hubService.connection.on('uploadDevice', (device) => {
            this.devices.push(
                new Device(
                    device.id,
                    device.name,
                    device.os,
                    device.version,
                    device.lastUpdate
                )
            );
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
        this.getEvents$.next(!this.automaticallyLoadEvents);
    }
}
