import { Subject } from 'rxjs';
import { ChannelEvent } from './event';

export class ChannelSubject {
    channel: string;
    subject: Subject<ChannelEvent>;
}