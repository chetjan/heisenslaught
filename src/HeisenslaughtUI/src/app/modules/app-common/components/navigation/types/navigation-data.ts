import { Route } from '@angular/router';

export interface NavigationData {
    label: string;
    path: string;
    showChildren: boolean;
    children?: NavigationConfig[];
    order: number;
}

export interface NavigationConfig {
    label: string;
    path?: string;
    showChildren?: boolean;
    children?: NavigationConfig[];
    order?: number;
}
