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

  @Input()
  public configs: NavigationConfig[];

  @Input()
  public baseRef: string;

  private openItems: boolean[] = [];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {

  }

  public isOpen(index: number): boolean {
    return !!this.openItems[index];
  }

  public toggleOpen(index: number) {
    this.openItems[index] = !this.openItems[index];
  }

  public open(index: number) {
    this.openItems[index] = true;
  }

  public close(index: number) {
    this.openItems[index] = false;
  }

  ngOnInit() {
    console.log('nav.configs', this.configs);
    if (!this.configs) {
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
      this.buildFromRoutes(routes, baseRef);
    } else {
      this.build(this.configs, this.baseRef);
    }
  }
  /*
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
  
          let navItem = {
            label: navConfig.label,
            path: path,
            config: configItem,
            showChildren: navConfig.showChildren || false,
            order: navConfig.order === undefined ? Number.MAX_VALUE : navConfig.order
          };
          if (navItem.showChildren) {
  
          }
  
          this.navItems.push(navItem);
  
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
    */

  private buildFromRoutes(routes: Route[], baseRef: string) {
    let configs: NavigationConfig[] = [];
    routes.forEach((route) => {
      if (route.data && route.data['navigation']) {
        let config: NavigationConfig = route.data['navigation'];
        config.path = route.path;
        configs.push(config);
      }
    });
    this.build(configs, baseRef);
  }

  private build(configs: NavigationConfig[], baseRef: string) {
    this.navItems = [];
    configs.forEach((config) => {
      let path = baseRef;
      if (config.path) {
        if (!path.endsWith('/')) {
          path += '/';
        }
        path += config.path;
      }
      let navItem = {
        label: config.label,
        path: path,
        showChildren: config.showChildren || false,
        children: config.children,
        order: config.order === undefined ? Number.MAX_VALUE : config.order
      };
      this.navItems.push(navItem);
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
