declare module MASFoundation {
    export enum ErrorCode {
        unknown,

        applicationAlreadyRegistered,

        applicationInvalid,

        applicationNotRegistered,

        applicationInvalidMagIdentifer,

        configurationLoadingFailedNullOrEmpty,

        configurationLoadingFailedFileNotFound,

        configurationLoadingFailedJsonSerialization,

        configurationLoadingFailedJsonValidation,

        configurationInvalidEndpoint,

        deviceAlreadyRegistered,

        deviceAlreadyRegisteredWithDifferentFlow,

        deviceCouldNotBeDeregistered,

        deviceNotRegistered,

        deviceNotLoggedIn,

        deviceRecordIsNotValid,

        deviceRegistrationAttemptedWithUnregisteredScope,

        networkNotReachable,

        responseSerializationFailedToParseResponse,

        userAlreadyAuthenticated,

        userBasicCredentialsNotValid,

        userNotAuthenticated,

        tokenAccessExpired,

        accessTokenInvalid,

        accessTokenDisabled,

        accessTokenNotGrantedScope,

        count
    }

    export class ErrorInfo {
        public code: ErrorCode;
        public text: string;
    }

    export interface IAddressInfo {
        region: string;
        country: string;
    }

    export interface IUserInfo {
        sub: string;
        name: string;
        familyName: string;
        nickname: string;
        perferredUsername: string;
        email: string;
        phone: string;
        address: IAddressInfo;
    }

    export enum LogLevel {
        none,

        full,

        errorOnly
    }

    export class MAS {
        public static configFileName: string;
        public static registrationKind: RegistrationKind;
        public static logLevel: LogLevel;

        //public static LoginRequested: Windows.Foundation.EventHandler<Object>;
        //public static LogMessage: Windows.Foundation.EventHandler<string>;

        public static addEventListener(type: string, listener: Windows.Foundation.EventHandler<any>): void;
        public static removeEventListener(type: string, listener: Windows.Foundation.EventHandler<any>): void;

        public static startAsync(): Windows.Foundation.IAsyncAction;

        public static startWithConfigAsync(configText: string): Windows.Foundation.IAsyncAction;

        public static resetAsync(): Windows.Foundation.IAsyncAction;

        public static errorLookup(number: number): ErrorInfo;

        public static deleteFromAsync(endPointPath: string,
            parameters: PropertyCollection,
            headers: PropertyCollection,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static getFromAsync(endPointPath: string,
            parameters: PropertyCollection,
            headers: PropertyCollection,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static postTextToAsync(endPointPath: string,
            text: string,
            headers: PropertyCollection,
            requestType: RequestType,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static postToAsync(endPointPath: string,
            parameters: PropertyCollection,
            headers: PropertyCollection,
            requestType: RequestType,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static patchTextToAsync(endPointPath: string,
            text: string,
            headers: PropertyCollection,
            requestType: RequestType,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static patchToAsync(endPointPath: string,
            parameters: PropertyCollection,
            headers: PropertyCollection,
            requestType: RequestType,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static putTextToAsync(endPointPath: string,
            text: string,
            headers: PropertyCollection,
            requestType: RequestType,
            responseType: ResponseType): Windows.Foundation.IAsyncOperation<TextResponse>;

        public static putToAsync(endPointPath: string,
            parameters: PropertyCollection,
            headers: PropertyCollection,
            requestType: RequestType,
            responseType: ResponseType): Windows.Foundation.IPromise<TextResponse>;
    }

    export class MASApplication {
        public static current: MASApplication;

        public static isRegistered: boolean;

        public organization: string;
        public name: string;
        public identifier: string;
        public detailedDescription: string;
        public environment: string;
        public redirectUri: string;
        public registeredBy: string;
        public scope: Windows.Foundation.Collections.IVectorView<string>;
        public scopeAsString: string;
        public status: string;
    }

    export class MASDevice {
        public static current: MASDevice;
        public id: string;
        public name: string;
        public isRegistered: boolean;
        public status: string;

        public unregisterAsync(): Windows.Foundation.IAsyncAction;
    }

    export class MASUser {

        public static current: MASUser;
        public isLoggedIn: boolean;

        public logoffAsync(): Windows.Foundation.IAsyncAction;

        public static loginAsync(username: string, password: string): Windows.Foundation.IAsyncOperation<MASUser>;

        public getInfoAsync(): Windows.Foundation.IAsyncOperation<IUserInfo>;

        public checkAccessAsync(): Windows.Foundation.IAsyncOperation<boolean>;

    }

    export class Property {
        public key: string;
        public value: string;
    }

    export class PropertyCollection {
        public add(key: string, value: string): void;
        public remove(key: string): void;
        public get(key: string): string;
        public getAt(index: number): Property;
        public count: number;
        public toString(): string;
    }

    export class ReadonlyPropertyCollection {
        public get(key: string): string;
        public getAt(index: number): Property;
        public count: number;
        public toString(): string;
    }

    export enum RegistrationKind {
        client,

        user
    }

    export enum RequestType {
        none,

        json,

        scimJson,

        plainText,

        formUrlEncoded,

        xml
    }

    export enum ResponseType {
        unknown,

        json,

        scimJson,

        plainText,

        xml
    }

    export class TextResponse {
        public headers: ReadonlyPropertyCollection;
        public text: string;
        public statusCode: number;
        public isSuccessful: boolean;
    }
}