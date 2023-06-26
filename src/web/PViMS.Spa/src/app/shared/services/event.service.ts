import { Injectable } from '@angular/core';
import * as Rx from 'rxjs';
import { from } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EventService {
    protected listeners: any;
    protected eventsSubject: any;
    protected events: any;

    constructor() {
        this.listeners = {};
        this.eventsSubject = new Rx.Subject();
        this.events = from(this.eventsSubject);

        this.events.subscribe(
            ({ name, args }) => {
                if (this.listeners[name])
                    for (let listener of this.listeners[name])
                        listener.function(...args);
            });
    }

    public CLog(object: any, title: string = undefined) {
        if (!environment.production)
            console.log({ title: title, object });
    }

    on(name, tag, listener) {
        if (!this.listeners[name])
            this.listeners[name] = [];

        if (this.listeners[name].map(e => e.tag).indexOf(tag) < 0) {
            var event = new EventClass();
            event.tag = tag;
            event.function = listener;
            this.listeners[name].push(event);
            if (!environment.production)
                console.log({ type: "register-event", name, tag });
        }
    }

    removeAll(tag) {
        for (let property of Object.keys(this.listeners))
            if (this.listeners[property].map(e => e.tag).indexOf(tag) >= 0)
                this.remove(property, tag);
    }

    remove(name, tag) {
        if (!this.listeners[name])
            return;

        let index = this.listeners[name].map(e => e.tag).indexOf(tag);
        if (index >= 0) {
            this.listeners[name].splice(index, 1);
            if (!environment.production)
                console.log({ type: "remove-event", name, tag });
        };
    }

    broadcast(name, ...args) {
        if (!environment.production && name !== 'tick-event')
            console.log({ type: "execute-event", name, args });
        this.eventsSubject.next({ name, args });
    }
}

class EventClass {
    tag: string;
    function: any;
}
