using Morph.Server.Sdk.Client;
using Morph.Server.Sdk.Model.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model.SessionErrorHandling
{
    /// <summary>
    /// Renewwed session
    /// </summary>
    public sealed class RenewedSession : SessionErrorHandlingStrategyResult
    {
        public RenewedSession(ApiSession newApiSession)
        {
            NewApiSession = newApiSession ?? throw new ArgumentNullException(nameof(newApiSession));
        }

        public ApiSession NewApiSession { get; }
    }

}
