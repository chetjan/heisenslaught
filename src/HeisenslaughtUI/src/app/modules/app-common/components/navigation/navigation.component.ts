import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { Router, ActivatedRoute, Route } from '@angular/router';
import { NavigationData, NavigationConfig } from './types/navigation-data';

export * from './types/navigation-data'

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent implements OnInit {

  public navItems: NavigationData[] = [];

  @Output()
  public navClick: EventEmitter<NavigationData> = new EventEmitter<NavigationData>();

  @Input()
  public useChildren: boolean;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {

  }

  ngOnInit() {
    let routes: Route[] = [];
    let baseRef = '';
    let config = this.activatedRoute.snapshot.routeConfig || this.router.config;
    if (!Array.isArray(config)) {
      baseRef = './';
      if (this.useChildren) {
        routes = config.children;
      } else {
        routes = [config];
      }
    } else {
      routes = config;
    }
    this.buildNav(routes, baseRef);
  }

  public buildNav(routes: Route[], baseRef: string) {
    this.navItems = [];
    routes.forEach((configItem) => {
      if (configItem.data && configItem.data['navigation']) {
        let navConfig: NavigationConfig = configItem.data['navigation'];
        let path = baseRef;
        if (configItem.path !== '') {
          if (!path.endsWith('/')) {
            path += '/';
          }
          path += configItem.path;
        }

        this.navItems.push({
          label: navConfig.label,
          path: path,
          config: configItem,
          showChildren: navConfig.showChildren || false,
          order: navConfig.order === undefined ? Number.MAX_VALUE : navConfig.order
        });

      }
    });
    this.navItems.sort((a, b) => {
      if (a.order > b.order) {
        return 1;
      }
      if (a.order < b.order) {
        return -1;
      }
      return a.label.localeCompare(b.label);
    });
  }

  public onItemClicked(item: NavigationData) {
    this.navClick.emit(item);
  }

}
