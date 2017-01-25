import { Route } from '@angular/router';
export interface NavigationData {
    label: string;
    path: string;
    config: Route;
    showChildren: boolean;
    order: number;
}

export interface NavigationConfig {
    label: string;
    showChildren?: boolean;
    order?: number;
}
