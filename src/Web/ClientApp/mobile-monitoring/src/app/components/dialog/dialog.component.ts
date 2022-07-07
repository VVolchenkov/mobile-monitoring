import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Event } from '../../../models/event';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

@Component({
    selector: 'app-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.scss'],
})
export class DialogComponent {
    form = new FormGroup({
        name: new FormControl({ value: this.event.name, disabled: true }),
        date: new FormControl({ value: this.event.date, disabled: true }),
        description: new FormControl(this.event.description),
    });

    constructor(
        private readonly formBuilder: FormBuilder,
        private readonly dialogRef: MatDialogRef<DialogComponent>,
        @Inject(MAT_DIALOG_DATA) public event: Event
    ) {}

    public save() {
        this.dialogRef.close(this.form.value);
    }
}
