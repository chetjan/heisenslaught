import { Pipe, PipeTransform } from '@angular/core';

import { HeroData } from '../../../heroes-data-service/heroes-data-service.module';


@Pipe({
    name: 'heroSearch'
})
export class HeroFilter implements PipeTransform {
    public transform(value: HeroData[], ...args: any[]): any {
        if (!Array.isArray(value)) {
            return null;
        }
        let result = value;

        // filter out disabled heroes
        let disabled: string[] = args[2];
        if (Array.isArray(disabled)) {
            result = result.filter((hero: HeroData) => {
                return disabled.indexOf(hero.id) === -1;
            });
        }
        // hero must match any selected role
        let roleFilters: string[] = args[1];
        if (Array.isArray(roleFilters) && roleFilters.length) {
            result = result.filter((hero: HeroData) => {
                for (let i = 0; i < hero.roles.length; i++) {
                    if (roleFilters.indexOf(hero.roles[i].toLowerCase()) !== -1) {
                        return true;
                    }
                }
            });
        }

        let tokens = this.tokenize((args[0] || '').toLowerCase());
        result = result.filter((hero: HeroData) => {
            let all = tokens.all;
            let any = tokens.any;
            let none = tokens.none;
            // hero must match all terms in all array
            if (all.length) {
                // any = any.concat(all);
                for (let i = 0; i < all.length; i++) {
                    if (!this.isMatch(all[i], hero)) {
                        return false;
                    }
                }
            }
            // hero must match at lease one term in any array
            if (any.length) {
                let match = false;
                for (let i = 0; !match && i < any.length; i++) {
                    if (this.isMatch(any[i], hero)) {
                        match = true;
                    }
                }
                if (!match) {
                    return false;
                }
            }
            // hero must not match any term in none array
            if (none.length) {
                for (let i = 0; i < none.length; i++) {
                    if (this.isMatch(none[i], hero)) {
                        return false;
                    }
                }
            }
            return true;
        });

        return result;
    }

    private isMatch(term: string, hero: HeroData): boolean {
        if (hero.name.toLowerCase().indexOf(term) !== -1) {
            return true;
        }
        if (hero.title.toLowerCase().indexOf(term) !== -1) {
            return true;
        }
        if (hero.franchise.toLowerCase().indexOf(term) !== -1) {
            return true;
        }
        for (let i = 0; i < hero.roles.length; i++) {
            let role = hero.roles[i].toLowerCase();
            if (role.indexOf(term) !== -1) {
                return true;
            }
        }
        for (let i = 0; i < hero.keywords.length; i++) {
            let keyword = hero.keywords[i].toLowerCase();
            if (keyword.indexOf(term) !== -1) {
                return true;
            }
        }
        return false;
    }

    private tokenize(searchString: string): { all: string[], any: string[], none: string[] } {
        let tokens = {
            all: [],
            any: [],
            none: []
        };
        let inQuotes = false;
        let token = '';
        let tokenType = 'any';
        let i;
        for (i = 0; i < searchString.length; i++) {
            let c = searchString.charAt(i);
            switch (c) {
                case ' ':
                    if (!inQuotes) {
                        if (token) {
                            tokens[tokenType].push(token);
                            token = '';
                            tokenType = 'any';
                        }
                    } else {
                        token += c;
                    }
                    break;
                case '"':
                    if (inQuotes) {
                        if (token) {
                            tokens[tokenType].push(token);
                            token = '';
                            tokenType = 'any';
                        }
                        inQuotes = false;
                    } else {
                        inQuotes = true;
                    }
                    break;
                case '+':
                    if (!inQuotes) {
                        if (token) {
                            tokens[tokenType].push(token);
                            token = '';
                        }
                        tokenType = 'all';
                    }
                    break;
                case '-':
                    if (!inQuotes) {
                        if (token) {
                            tokens[tokenType].push(token);
                            token = '';
                        }
                        tokenType = 'none';
                    }
                    break;
                default:
                    token += c;
                    break;
            }
        }

        if (token) {
            tokens[tokenType].push(token);
        }
        return tokens;
    }
}

