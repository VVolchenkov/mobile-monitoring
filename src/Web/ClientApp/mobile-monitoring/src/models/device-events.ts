import { Event } from './event';

export class DeviceEvents {

    constructor(
        public readonly id: string,
        public readonly events: Event[],
) {}
}
