// <copyright file="axios-decorator.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios, { AxiosResponse, AxiosRequestConfig } from "axios";
import Resources from "../constants/resources";
import * as microsoftTeams from "@microsoft/teams-js";

/**
* Average network timeout in milliseconds.
*/
axios.defaults.timeout = Resources.axiosDefaultTimeout;

/**
* Application base URI.
*/
axios.defaults.baseURL = window.location.origin;

class AxiosJWTDecorator {

    /**
    * Delete data
    * @param  {String} url Resource URI
    * @param  {any | Null} data Request data
    */
    public async delete<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        needAuthorizationHeader: boolean = true
    ): Promise<R> {
        try {
            let config: AxiosRequestConfig = axios.defaults;
            if (needAuthorizationHeader) {
                config = await this.setupAuthorizationHeader(config);
            }
            if (data) {
                config.headers["Content-Type"] = 'application/json; charset=utf-8';
                config.data = data;
            }

            return await axios.delete(url, config);
        } catch (error) {
            return error.response;
        }
    }

    /**
    * Post data to API
    * @param  {String} url Resource URI
    * @param  {Object} data Request body data
    */
    public async post<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        needAuthorizationHeader: boolean = true
    ): Promise<R> {
        try {
            let config: AxiosRequestConfig = axios.defaults;
            if (needAuthorizationHeader) {
                config = await this.setupAuthorizationHeader(config!);
            }

            return await axios.post(url, data, config);
        } catch (error) {
            return error.response;
        }
    }

    /**
    * Update data with patch request
    * @param  {String} url Resource URI
    * @param  {Object} data Request body data
    */
    public async patch<T = any, R = AxiosResponse<T>>(
        url: string,
        data?: any,
        needAuthorizationHeader: boolean = true
    ): Promise<R> {
        try {
            let config: AxiosRequestConfig = axios.defaults;
            if (needAuthorizationHeader) {
                config = await this.setupAuthorizationHeader(config!);
            }

            return await axios.patch(url, data, config);
        } catch (error) {
            return error.response;
        }
    }

    /**
    * Get data from API
    * @param  {String} url Resource URI
    * @param  {Function} handleAuthFailure Authentication failure callback function
    */
    public async get<T = any, R = AxiosResponse<T>>(
        url: string,
        handleAuthFailure?: (error: string) => void,
        needAuthorizationHeader: boolean = true
    ): Promise<R> {
        try {
            let config: AxiosRequestConfig = axios.defaults;
            config = axios.defaults;
            if (needAuthorizationHeader) {
                config = await this.setupAuthorizationHeader(config, handleAuthFailure);
            }

            return await axios.get(url, config);
        } catch (error) {
            return error.response;
        }
    }

    /**
    * Sets authorization header in request.
    * @param  {String} config Axios request configuration details
    * @param  {Function} handleAuthFailure Authentication failure callback function
    */
    private async setupAuthorizationHeader(
        config: AxiosRequestConfig,
        handleFailure: (error: string) => void = (error: string) => { console.error("Error from getAuthToken: ", error); }
    ): Promise<AxiosRequestConfig> {
        microsoftTeams.initialize();

        return new Promise<AxiosRequestConfig>((resolve, reject) => {
            const authTokenRequest = {
                successCallback: (token: string) => {
                    if (!config) {
                        config = axios.defaults;
                    }
                    config.headers["Authorization"] = `Bearer ${token}`;
                    resolve(config);
                },
                failureCallback: handleFailure,
                resources: []
            };
            microsoftTeams.authentication.getAuthToken(authTokenRequest);
        });
    }
}

const axiosJWTDecoratorInstance = new AxiosJWTDecorator();
export default axiosJWTDecoratorInstance;