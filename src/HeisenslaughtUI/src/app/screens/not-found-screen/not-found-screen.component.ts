import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-found-screen',
  template: ''
})
export class NotFoundScreenComponent {

  constructor(private router: Router) {
    let url: string = router.url;
    if (router.url.indexOf('/404') !== 0) {
      router.navigate(['/404' + router.url], { replaceUrl: true });
    } else {
      url = url.substr(3);
    }
    window.location.replace(url);
  }

}
