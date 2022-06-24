import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {AppComponent} from './app.component';
import {FormsModule} from '@angular/forms';
import {ApiService} from '../services/api.service';
import {IApiService} from '../interfaces/api-service';
import {HttpClientModule} from '@angular/common/http';

@NgModule({
    declarations: [AppComponent],
    imports: [BrowserModule, FormsModule, HttpClientModule],
    providers: [
        {
            provide: IApiService,
            useClass: ApiService,
        },
    ],
    exports: [],
    bootstrap: [AppComponent],
})
export class AppModule {
}
