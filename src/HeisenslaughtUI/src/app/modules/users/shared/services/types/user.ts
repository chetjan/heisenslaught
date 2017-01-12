export interface AuthenticatedUser {
    username: string;
    usernameNormailzed: string;
    requiresSetup: string;
    requiresEmailValidation: string;
    roles: string[];
}
