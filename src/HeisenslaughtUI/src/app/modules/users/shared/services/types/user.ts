export interface AuthenticatedUser {
    id: string;
    username: string;
    usernameNormailzed: string;
    requiresSetup: string;
    requiresEmailValidation: string;
    roles: string[];
}
