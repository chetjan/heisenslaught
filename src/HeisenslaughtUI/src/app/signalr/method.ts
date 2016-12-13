export class MethodCall<T extends Function> {
    methodName: string;
    method: T;
}
