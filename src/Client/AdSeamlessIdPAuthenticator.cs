using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Helper;
using Morph.Server.Sdk.Model;
using Morph.Server.Sdk.Model.InternalModels;
using System;
using System.Collections.Specialized;
using System.Net.Http;

using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Client
{
    internal static class AdSeamlessIdPAuthenticator
    {
        public static async Task<ApiSession> OpenSession(OpenSessionAuthenticatorContext context, string idPId, bool keepSignedId, CancellationToken cancellationToken)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrEmpty(idPId))
            {
                throw new ArgumentException($"'{nameof(idPId)}' cannot be null or empty.", nameof(idPId));
            }


            // handler will be disposed automatically
            using (HttpClientHandler aHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic,
                // required for automatic NTML/Negotiate challenge
                UseDefaultCredentials = true,
                ServerCertificateCustomValidationCallback = context.MorphServerApiClient.Config.ServerCertificateCustomValidationCallback
            })
            {
                // build a new low level client based on specified handler
                using (var ntmlRestApiClient = context.BuildApiClient(aHandler))
                {
                    var serverNonceApiResult = await context.LowLevelApiClient.AuthGenerateNonce(cancellationToken);
                    serverNonceApiResult.ThrowIfFailed();
                    var nonce = serverNonceApiResult.Data.Nonce;

                    var token = await internalAuthExternalWindowAsync(ntmlRestApiClient, nonce, idPId, keepSignedId, cancellationToken);

                    return ApiSessionFactory.CreatePersitableApiSession(token);
                }
            }
        }
        static async Task<string> internalAuthExternalWindowAsync(IRestClient apiClient, string serverNonce, string idPId, bool keepSignedIn, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idPId))
            {
                throw new ArgumentException($"'{nameof(idPId)}' cannot be null or whitespace.", nameof(idPId));
            }

            var url = UrlHelper.JoinUrl("auth", "external", "windows", idPId);
            var requestDto = new AdSeamlessIdPSigninRequestDto
            {
                RequestToken = serverNonce
            };

            var ulrParameters = new NameValueCollection
            {
                {"keepSignedIn", keepSignedIn.ToString() }
            };
            var apiResult = await apiClient.PostAsync<AdSeamlessIdPSigninRequestDto, LoginResponseDto>(url, requestDto, ulrParameters, new HeadersCollection(), cancellationToken);
            apiResult.ThrowIfFailed();
            return apiResult.Data.Token;

        }
    }
}


