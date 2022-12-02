import axios from "axios";
import { redirect, useOutletContext } from "react-router-dom";
import { formatRoute, Routes } from "./apiRoutes";

const USER_KEY = "TVShowApplication.User";

type Role = "Admin" | "Poster" | "User";

interface User {
    Id: number,
    Role: Role,
    Email: string,
    AccessToken: string,
    RefreshToken: string,
}

type AccessTokenPayload = {
    Role: Role,
    Id: number,
    ExpiresAt: number,
    Issuer: string,
    Audience: string,
}

function parseJwt(token: string): AccessTokenPayload {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    const obj = JSON.parse(jsonPayload);
    const accessTokenPayload = {
        Role: obj.role,
        Id: obj.nameid,
        ExpiresAt: obj.exp,
        Issuer: obj.iss,
        Audience: obj.aud,
    };

    return accessTokenPayload;
}

type LoginRequest = {
    Email: string,
    Password: string,
}

type RegisterRequest = {
    Email: string,
    Password: string,
    RoleSecret: string,
}

type RefreshTokenRequest = {
    AccessToken: string,
    RefreshToken: string,
}

type AuthenticatedResponse = {
    accessToken: string,
    refreshToken: string,
}

interface AuthenticationManager {
    getUser(): User | null,
    setUser(userData: User): any,
    isAuthenticated(): boolean,

    login(request: LoginRequest): Promise<boolean>,
    register(request: RegisterRequest): Promise<boolean>,
    refreshToken(request: RefreshTokenRequest): Promise<boolean>,
    signOut(): Promise<boolean>,
}

class LocalStorageAuthenticationManager implements AuthenticationManager {
    user: User | null;
    setUserState: any;

    constructor(user: User | null, setUser: any) {
        this.user = user;
        this.setUserState = setUser;
    }

    async login(request: LoginRequest): Promise<boolean> {
        const route = Routes.GetToken;

        try {
            const getTokenResponse = await axios.post<AuthenticatedResponse>(route, request);
            if (getTokenResponse.status != 200
                || !getTokenResponse.data.accessToken
                || !getTokenResponse.data.refreshToken) return false;

            const accessTokenPayload = parseJwt(getTokenResponse.data.accessToken);
            const userData: User = {
                Id: accessTokenPayload.Id,
                Role: accessTokenPayload.Role,
                Email: request.Email,
                AccessToken: getTokenResponse.data.accessToken,
                RefreshToken: getTokenResponse.data.refreshToken,
            };

            this.setUser(userData);
        } catch (e) {
            console.error(e);

            return false;
        }

        return true;
    }

    async register(request: RegisterRequest): Promise<boolean> {
        const route = Routes.SignUp;

        try {
            const response = await axios.post(route, request);

            return true;
        } catch (e) {
            console.error(e);

            return false;
        }
    }

    refreshToken(request: RefreshTokenRequest): Promise<boolean> {
        throw new Error("Method not implemented.");
    }

    signOut(): Promise<boolean> {
        throw new Error("Method not implemented.");
    }

    isAuthenticated(): boolean {
        return this.user !== null;
    }

    setUser(userData: User) {
        localStorage.setItem(USER_KEY, JSON.stringify(userData));
        this.setUserState(userData);
    }

    getUser(): User | null {
        return this.user;
    }

    loadFromLocalStorage() {
        const localStorageUser = localStorage.getItem(USER_KEY);

        if (!localStorageUser || this.user != null) return;

        const userJson = JSON.parse(localStorageUser);
        this.setUserState(userJson);
    }
}

type AuthenticationContext = { authenticationManager: AuthenticationManager }

export { LocalStorageAuthenticationManager };
export type { User, AuthenticationManager, AuthenticationContext };
