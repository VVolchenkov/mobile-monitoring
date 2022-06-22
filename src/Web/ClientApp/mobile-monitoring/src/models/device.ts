import { Application } from './application';

export class Device {
    constructor(
        public readonly id: number,
        public readonly fullName: string,
        public readonly platform: string,
        public readonly application: Application,
        public readonly lastUpdate: Date
    ) {}
}
