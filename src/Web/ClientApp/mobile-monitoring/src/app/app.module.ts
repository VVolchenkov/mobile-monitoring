import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {AppComponent} from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {ApiService} from '../services/api.service';
import {IApiService} from '../interfaces/api-service';
import {HttpClientModule} from '@angular/common/http';
import {HubService} from '../services/hub.service';
import { MatDialogModule } from '@angular/material/dialog';
import { DialogComponent } from './components/dialog/dialog.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule } from '@angular/material/input';

@NgModule({
    declarations: [AppComponent, DialogComponent],
    imports: [BrowserModule, FormsModule, HttpClientModule, MatDialogModule, BrowserAnimationsModule, MatInputModule, ReactiveFormsModule],
    providers: [
        {
            provide: IApiService,
            useClass: ApiService,
        },
        HubService,
    ],
    exports: [],
    bootstrap: [AppComponent],
    entryComponents: [DialogComponent]
})
export class AppModule {
}
