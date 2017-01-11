export class LoginWindow {

    private _url: string;
    private _width: number;
    private _height: number;
    private _window: Window;

    public constructor(url: string, width: number, height: number) {
        this._url = url;
        this._width = width;
        this._height = height;
        window.addEventListener('unload', () => {
            this.close();
        });
    }


    public get isOpen(): boolean {
        return this._window ? !this._window.closed : false;
    }

    public open(returnUrl = '/'): void {
        if (!this.isOpen) {
            this._window = this.openPopup(this._url + '&returnUrl=' + returnUrl, this._width, this._height);
        }
        this.focus();
    }

    public close(): void {
        if (this.isOpen) {
            this._window.close();
            this._window = null;
        }
    }

    public focus(): void {
        if (this.isOpen && this._window.focus) {
            this._window.focus();
        }
    }

    private openPopup(url: string, pWidth: number, pHeight: number): Window {
        let screenOffsetX = window.screenLeft !== undefined ? window.screenLeft : window.screenX;
        let screenOffsetY = window.screenTop !== undefined ? window.screenTop : window.screenY;

        let screenW = window.screen.availWidth;
        let screenH = window.screen.availHeight;

        let left = (screenW / 2) - (pWidth / 2) + screenOffsetX;
        let top = (screenH / 2) - (pHeight / 2) + screenOffsetY;

        let options = 'left=' + left + ',top=' + top + ',width=' + pWidth + ',height=' + pHeight;
        return window.open(url, undefined, options);
    }


}

