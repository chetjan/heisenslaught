import { Pipe, PipeTransform } from '@angular/core';
import { HeroData } from '../services/hero';

@Pipe({
    name: 'heroSearch'
})
export class HeroFilter implements PipeTransform {
    public transform(value: HeroData[], ...args: any[]): any {
        let text: string = args[0] ? args[0].toLowerCase() : undefined;
        let roles: string[] = args[1];

        let result: HeroData[] = value && roles && roles.length ? value.filter((hero: HeroData) => {
            for (let i = 0; i < hero.roles.length; i++) {
                let keyword = hero.roles[i].toLowerCase();
                if (roles.indexOf(keyword) !== -1) {
                    console.log('found', keyword);
                    return true;
                }
            }
            return false;
        }) : value;

        result = value && text ? result.filter((hero: HeroData) => {
            if (hero.name.toLowerCase().indexOf(text) !== -1) {
                return true;
            }

            if (hero.keywords && hero.keywords.length) {
                for (let i = 0; i < hero.keywords.length; i++) {
                    let keyword = hero.keywords[i].toLowerCase();
                    if (keyword.indexOf(text) !== -1) {
                        return true;
                    }
                }
            }

            if (hero.roles && hero.roles.length) {
                for (let i = 0; i < hero.roles.length; i++) {
                    let keyword = hero.roles[i].toLowerCase();
                    if (keyword.indexOf(text) !== -1) {
                        return true;
                    }
                }
            }
            return false;
        }) : result;
        return result;
    }
}

