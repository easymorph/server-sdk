﻿using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Helper;
using Morph.Server.Sdk.Model;
using Morph.Server.Sdk.Model.InternalModels;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;

using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Client
{


    internal static class MorphServerLegacyAuthenticator
    {

        public static async Task<ApiSession> OpenLegacySessionMultiplexedAsync(
            SpaceEnumerationItem desiredSpace,
            OpenSessionAuthenticatorContext context,
            OpenLegacySessionRequest openSessionRequest,
            CancellationToken cancellationToken)
        {
            
            if (desiredSpace.SpaceAuthenticationProviderTypes.Contains(IdPType.SpacePwd))
            {
                // password protected space                                
                return await OpenSessionViaSpacePasswordAsync(context, openSessionRequest.SpaceName, openSessionRequest.Password, cancellationToken);
            }
            else if (desiredSpace.SpaceAuthenticationProviderTypes.Contains(IdPType.Anonymous))
            {
                return ApiSessionFactory.CreateAnonymousSession();
            }
            else if (desiredSpace.SpaceAuthenticationProviderTypes.Contains(IdPType.AdSeamlessIdP))
            {
                // windows authentication            
                return await OpenSessionViaWindowsAuthenticationAsync(context, openSessionRequest.SpaceName, cancellationToken);                 
            }
            else {                
               throw new Exception("Space access authentification method is not supported by this client.");
            }
        }


        static async Task<ApiSession> OpenSessionViaWindowsAuthenticationAsync(OpenSessionAuthenticatorContext context, string spaceName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(spaceName))
            {
                throw new ArgumentException("Space name is not set", nameof(spaceName));
            }
            // handler will be disposed automatically
            HttpClientHandler aHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic,
                // required for automatic NTML/Negotiate challenge
                UseDefaultCredentials = true,
                ServerCertificateCustomValidationCallback = context.MorphServerApiClient.Config.ServerCertificateCustomValidationCallback
            };

            // build a new low level client based on specified handler
            using (var ntmlRestApiClient = context.BuildApiClient(aHandler))
            {
                var serverNonce = await internalGetAuthNonceAsync(ntmlRestApiClient, cancellationToken);
                var token = await internalAuthExternalWindowAsync(ntmlRestApiClient, spaceName, serverNonce, cancellationToken);

                return ApiSessionFactory.CreateLegacySession(context.MorphServerApiClient, spaceName, token);
            }
        }
        static async Task<string> internalGetAuthNonceAsync(IRestClient apiClient, CancellationToken cancellationToken)
        {
            var url = "auth/nonce";
            var response = await apiClient.PostAsync<GenerateNonceRequestDto, GenerateNonceResponseDto>
                (url, new GenerateNonceRequestDto(), null, new HeadersCollection(), cancellationToken);
            response.ThrowIfFailed();
            return response.Data.Nonce;
        }

        static async Task<string> internalAuthExternalWindowAsync(IRestClient apiClient, string spaceName, string serverNonce, CancellationToken cancellationToken)
        {
            var url = "auth/external/windows";
            var requestDto = new WindowsExternalLoginRequestDto
            {
                RequestToken = serverNonce,
                SpaceName = spaceName
            };

            var apiResult = await apiClient.PostAsync<WindowsExternalLoginRequestDto, LoginResponseDto>(url, requestDto, null, new HeadersCollection(), cancellationToken);
            apiResult.ThrowIfFailed();
            return apiResult.Data.Token;

        }



        /// <summary>
        /// Open a new authenticated session via password
        /// </summary>
        /// <param name="spaceName">space name</param>
        /// <param name="password">space password</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        static async Task<ApiSession> OpenSessionViaSpacePasswordAsync(OpenSessionAuthenticatorContext context, string spaceName, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(spaceName))
            {
                throw new ArgumentException("Space name is not set.", nameof(spaceName));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var passwordSha256 = CryptographyHelper.CalculateSha256HEX(password);
            var serverNonceApiResult = await context.LowLevelApiClient.AuthGenerateNonce(cancellationToken);
            serverNonceApiResult.ThrowIfFailed();
            var serverNonce = serverNonceApiResult.Data.Nonce;
            var clientNonce = ConvertHelper.ByteArrayToHexString(CryptographyHelper.GenerateRandomSequence(16));
            var all = passwordSha256 + serverNonce + clientNonce;
            var composedHash = CryptographyHelper.CalculateSha256HEX(all);


            var requestDto = new LoginRequestDto
            {
                ClientSeed = clientNonce,
                Password = composedHash,
                Provider = "Space",
                UserName = spaceName,
                RequestToken = serverNonce
            };
            var authApiResult = await context.LowLevelApiClient.AuthLoginPasswordAsync(requestDto, cancellationToken);
            authApiResult.ThrowIfFailed();
            var token = authApiResult.Data.Token;


            return ApiSessionFactory.CreateLegacySession(context.MorphServerApiClient, spaceName, token);

        }



    }
}


