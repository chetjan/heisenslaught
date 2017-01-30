import { Http, RequestOptionsArgs } from '@angular/http';
export abstract class RestService {
  protected _baseUrl: string;

  constructor(protected http: Http) { }

  protected get baseUrl(): string {
    return '';
  }

  protected get(path: string, options?: RequestOptionsArgs): Promise<any> {
    return this.http.get(this.getUrl(path), options).map(res => res.json()).toPromise();
  }

  protected delete(path: string, options?: RequestOptionsArgs): Promise<any> {
    return this.http.delete(this.getUrl(path), options).map(res => res.json()).toPromise();
  }

  protected head(path: string, options?: RequestOptionsArgs): Promise<any> {
    return this.http.head(this.getUrl(path), options).map(res => res.json()).toPromise();
  }

  protected options(path: string, options?: RequestOptionsArgs): Promise<any> {
    return this.http.options(this.getUrl(path), options).map(res => res.json()).toPromise();
  }

  protected put(path: string, data: any, options?: RequestOptionsArgs): Promise<any> {
    return this.http.put(this.getUrl(path), data, options).map(res => res.json()).toPromise();
  }

  protected patch(path: string, data: any, options?: RequestOptionsArgs): Promise<any> {
    return this.http.patch(this.getUrl(path), data, options).map(res => res.json()).toPromise();
  }

  protected post(path: string, data: any, options?: RequestOptionsArgs): Promise<any> {
    return this.http.post(this.getUrl(path), data, options).map(res => res.json()).toPromise();
  }

  protected getUrl(partailUrl: string): string {
    return (this.baseUrl ? this.baseUrl + '/' : '') + partailUrl;
  }

}
