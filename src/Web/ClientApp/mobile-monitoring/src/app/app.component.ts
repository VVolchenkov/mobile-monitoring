import { Component, OnDestroy, OnInit } from '@angular/core';
import { IApiService } from '../interfaces/api-service';
import { Device } from '../models/device';
import {
    BehaviorSubject,
    combineLatest,
    filter,
    interval,
    map,
    of,
    Subject,
    switchMap,
    takeUntil,
    tap,
} from 'rxjs';
import { Event } from '../models/event';
import { HubService } from '../services/hub.service';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { DialogComponent } from './components/dialog/dialog.component';

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
    automaticallyLoadEvents = false;
    eventsSubject$: BehaviorSubject<Event[]> = new BehaviorSubject<Event[]>([]);

    constructor(
        private readonly apiService: IApiService,
        private readonly hubService: HubService,
        private readonly matDialog: MatDialog
    ) {}

    public ngOnInit(): void {
        this.apiService
            .getDevices()
            .pipe(takeUntil(this.componentDestroyed$))
            .subscribe((response) => {
                this.devices = response;
            });

        combineLatest([this.getEvents$, this.selectedDeviceId$])
            .pipe(
                switchMap(([getEvents, id]) =>
                    getEvents ? interval(3000).pipe(map((_) => id)) : of(id)
                ),
                switchMap((x) =>
                    x ? this.apiService.getDeviceEvents(x) : of({ events: [] })
                ),
                map((x) => x.events),
                tap((events) => this.eventsSubject$.next(events)),
                takeUntil(this.componentDestroyed$)
            )
            .subscribe(this.eventsSubject$);

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

    public setSelectedDevice(device: Device): void {
        const selectedDevice = this.devices.find((x) => x.id === device.id);

        if (selectedDevice) {
            this.selectedDevice = selectedDevice;
            this.selectedDeviceId$.next(selectedDevice.id);
        }
    }

    public setGetEvents(): void {
        this.getEvents$.next(!this.automaticallyLoadEvents);
    }

    public deleteEvents(): void {
        if (this.selectedDevice) {
            this.apiService
                .deleteDeviceEvents(this.selectedDevice.id)
                .subscribe(() => this.eventsSubject$.next([]));
        }
    }

    public isSelected(device: Device): boolean {
        return this.selectedDevice === device;
    }

    public openModal(selectedDeviceId: string, event: Event): void {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.autoFocus = true;
        dialogConfig.data = event;
        const dialogRef = this.matDialog.open(DialogComponent, dialogConfig);
        dialogRef
            .afterClosed()
            .pipe(filter((x) => x))
            .subscribe((updatedEvent) => {
                event.description = updatedEvent.description;
                this.apiService.updateDeviceEvent(selectedDeviceId, event).subscribe(() => console.log('updated event'));
            });
    }
}
