export class Event {
    constructor(
        public readonly id: string,
        public readonly name: string,
        public description: string,
        public readonly date: Date,
        public readonly deviceId: string
    ) {}
}
