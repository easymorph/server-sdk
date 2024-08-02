using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Helper;
using Morph.Server.Sdk.Model;
using Morph.Server.Sdk.Model.InternalModels;
using System;

using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Client
{
    internal static class InternalIdPAuthenticator
    {

        public static async Task<ApiSession> OpenSessionUserPasswordAsync(OpenSessionAuthenticatorContext context, string userName, string password, bool keepSignedIn, CancellationToken cancellationToken)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var passwordSha256 = CryptographyHelper.CalculateSha256HEX(password);
            var serverNonceApiResult = await context.LowLevelApiClient.AuthGenerateNonce(cancellationToken);
            serverNonceApiResult.ThrowIfFailed();
            var serverNonce = serverNonceApiResult.Data.Nonce;
            var clientNonce = ConvertHelper.ByteArrayToHexString(CryptographyHelper.GenerateRandomSequence(32));
            var all = passwordSha256 + serverNonce + clientNonce;
            var composedHash = CryptographyHelper.CalculateSha256HEX(all);


            var requestDto = new LoginRequestDto
            {
                ClientSeed = clientNonce,
                Password = composedHash,
                Provider = "LocalUser",
                UserName = userName,
                RequestToken = serverNonce,
                KeepSignedIn = keepSignedIn
            };
            var authApiResult = await context.LowLevelApiClient.AuthLoginPasswordAsync(requestDto, cancellationToken);
            authApiResult.ThrowIfFailed();
            var token = authApiResult.Data.Token;


            return ApiSessionFactory.CreatePersitableApiSession(token);

        }


    }
}


