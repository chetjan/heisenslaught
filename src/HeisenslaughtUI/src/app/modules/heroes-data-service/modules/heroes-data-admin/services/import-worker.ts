import { EventEmitter } from '@angular/core';
import { ASYNC_HELPERS, _httpPost, _importImage, WORKER_ONMESSAGE } from './import-worker-helpers';

export { _httpPost, _importImage };

export class ImportWebWorker {
    private _worker: Worker;
    public working = false;

    public data: EventEmitter<any> = new EventEmitter();
    constructor(workFn: Function) {
        let wrkBody = ASYNC_HELPERS +
            'var baseRef = "' + window.location.protocol + '//' + window.location.host + '";\n' +
            _importImage + '\n' +
            'var doWork = ' + workFn.toString() + '\n' +
            WORKER_ONMESSAGE;
        console.log(wrkBody)
        let wrkBlob = new Blob([wrkBody], { type: 'text/javascript' });
        this._worker = new Worker(window.URL.createObjectURL(wrkBlob));
        this._worker.addEventListener('message', (event) => {
            if (event.data === 'done') {
                this.working = false;
            }
            this.data.emit(event.data);
        });
    }

    public doWork(data: any) {
        this.working = true;
        this._worker.postMessage(data);
    }
}

