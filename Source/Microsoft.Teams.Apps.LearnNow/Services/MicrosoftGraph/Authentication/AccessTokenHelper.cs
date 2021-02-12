// <copyright file="AccessTokenHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.LearnNow.Services.MicrosoftGraph.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using Microsoft.Teams.Apps.LearnNow.Common;
    using Microsoft.Teams.Apps.LearnNow.Helpers;
    using Microsoft.Teams.Apps.LearnNow.Models.Configuration;

    /// <summary>
    /// Gets access token to access Microsoft Graph api.
    /// </summary>
    public class AccessTokenHelper : ITokenHelper
    {
        /// <summary>
        /// Instance of IOptions to read data from azure application configuration.
        /// </summary>
        private readonly IOptions<AzureActiveDirectorySettings> azureAdOptions;

        /// <summary>
        /// Instance of IOptions to read data from tenant configuration.
        /// </summary>
        private readonly IOptions<BotSettings> botSettings;

        /// <summary>
        /// Sends logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<AccessTokenHelper> logger;

        /// <summary>
        /// Instance of confidential client applications to access Web API.
        /// </summary>
        private readonly IConfidentialClientApplication confidentialClientApp;

        /// <summary>
        /// Instance of token acquisition helper to access token.
        /// </summary>
        private readonly TokenAcquisitionHelper tokenAcquisitionHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenHelper"/> class.
        /// </summary>
        /// <param name="azureAdOptions">Instance of IOptions to read data from application configuration.</param>
        /// <param name="botSettings">Instance of IOptions to read data tenant details.</param>
        /// <param name="confidentialClientApp">Instance of ConfidentialClientApplication class.</param>
        /// <param name="tokenAcquisitionHelper">Instance of token acquisition helper to access token.</param>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        public AccessTokenHelper(
            IOptions<AzureActiveDirectorySettings> azureAdOptions,
            IOptions<BotSettings> botSettings,
            IConfidentialClientApplication confidentialClientApp,
            TokenAcquisitionHelper tokenAcquisitionHelper,
            ILogger<AccessTokenHelper> logger)
        {
            this.azureAdOptions = azureAdOptions;
            this.botSettings = botSettings;
            this.confidentialClientApp = confidentialClientApp;
            this.logger = logger;
            this.tokenAcquisitionHelper = tokenAcquisitionHelper;
        }

        /// <summary>
        /// Get user Azure AD access token.
        /// </summary>
        /// <param name="userObjectId">User object id.</param>
        /// <param name="authorizationHeader">HttpRequest authorization headers.</param>
        /// <returns>Access token with Graph scopes.</returns>
        public async Task<string> GetAccessTokenAsync(string userObjectId, string authorizationHeader)
        {
            if (string.IsNullOrEmpty(userObjectId))
            {
                throw new ArgumentException("userObjectId is null", nameof(userObjectId));
            }

            List<string> scopeList = this.azureAdOptions.Value.GraphScope.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            try
            {
                // Gets user account from the accounts available in token cache.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.clientapplicationbase.getaccountasync?view=azure-dotnet
                // Concatenation of UserObjectId and TenantId separated by a dot is used as unique identifier for getting user account.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.accountid.identifier?view=azure-dotnet#Microsoft_Identity_Client_AccountId_Identifier
                var account = await this.confidentialClientApp.GetAccountAsync($"{userObjectId}.{this.botSettings.Value.TenantId}");

                // Attempts to acquire an access token for the account from the user token cache.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.clientapplicationbase.acquiretokensilent?view=azure-dotnet
                AuthenticationResult result = await this.confidentialClientApp
                    .AcquireTokenSilent(scopeList, account)
                    .ExecuteAsync();

                return result.AccessToken;
            }
            catch (MsalUiRequiredException ex)
            {
                try
                {
                    // Getting new token using AddTokenToCacheFromJwtAsync as AcquireTokenSilent failed to load token from cache.
                    this.logger.LogInformation($"MSAL exception occurred and trying to acquire new token. MSAL exception details are found {ex}.");
                    var jwtToken = AuthenticationHeaderValue.Parse(authorizationHeader).Parameter;
                    var scheme = AuthenticationHeaderValue.Parse(authorizationHeader).Scheme;
                    if (scheme != Constants.BearerAuthorizationScheme)
                    {
                        this.logger.LogError($"Authentication scheme : {scheme} is not valid.");
                        return null;
                    }

                    return await this.tokenAcquisitionHelper.AddTokenToCacheFromJwtAsync(this.azureAdOptions.Value.GraphScope, jwtToken);
                }
                catch (MsalException msalex)
                {
                    this.logger.LogError(msalex, $"An error occurred in GetAccessTokenAsync: {msalex.Message}.");
                }

                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in fetching token : {ex.Message}.");
                throw;
            }
        }
    }
}
