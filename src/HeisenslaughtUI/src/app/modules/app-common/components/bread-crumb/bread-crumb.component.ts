import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { Router, ActivatedRoute, Route, ActivatedRouteSnapshot } from '@angular/router';
import { NavigationConfig, NavigationData } from '../navigation/types/navigation-data';

@Component({
  selector: 'app-bread-crumb',
  templateUrl: './bread-crumb.component.html',
  styleUrls: ['./bread-crumb.component.scss']
})
export class BreadCrumbComponent implements OnInit {
  public current: NavigationData;
  public parents: NavigationData[];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    activatedRoute.params.subscribe((params) => {
      this.update(params);
    });
  }

  ngOnInit() {
  }

  private update(params) {
    console.log('activatedRoute', this.activatedRoute);
    this.build(this.activatedRoute.snapshot);
  }

  private build(route: ActivatedRouteSnapshot) {
    let configs: NavigationConfig[] = [];
    while (route) {
      let rCfg = route.routeConfig;
      if (rCfg && rCfg.data && rCfg.data['navigation']) {
        let cfg: NavigationConfig = rCfg.data['navigation'];
        configs.unshift(cfg);
      } else if (route.parent) {
        let pConfig = route.parent.routeConfig;
        if (pConfig && pConfig.data && pConfig.data['navigation']) {
          let pCfg: NavigationConfig = pConfig.data['navigation'];
          if (pCfg.children) {
            console.log('pCfg.children', pCfg.children, rCfg ? rCfg.path : undefined);
            let cfg = pCfg.children.find(item => {
              return rCfg ? item.path === rCfg.path : false;
            });
            if (cfg) {
              configs.unshift(cfg);
            }
          }
        }
      }
      route = route.parent;
    }

    let data: NavigationData[] = [];
    let path = '';
    configs.forEach(item => {
      path += '/' + item.path;
      let navItem: NavigationData = {
        path: path,
        label: item.label,
        order: data.length,
        showChildren: item.showChildren,
        children: item.children
      };
      data.push(navItem);
    });
    this.current = data.pop();
    this.parents = data;
  }

}
