import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'not-found-screen',
  templateUrl: './not-found-screen.component.html',
  styleUrls: ['./not-found-screen.component.scss']
})
export class NotFoundScreenComponent {

  constructor(private router: Router, private route: ActivatedRoute) {
    route.url.subscribe((urlSeg) => {
      let url = router.url;
      // if link begins with '/ext/' let the server handle it
      if (url.indexOf('/ext/') === 0) {
        window.location.replace(url.substr(4));
      }
    });
  }
}
