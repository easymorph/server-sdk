using Morph.Server.Sdk.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model
{
   

    public abstract class ApiSession : IDisposable
    {
        public const string AuthHeaderName = "X-EasyMorph-Auth";

        /// <summary>
        /// A unique guid that persistently identifies the object.
        /// </summary>
        public Guid LocalIdentifier { get; protected set; }

        /// <summary>
        /// Auth secret
        /// </summary>
        public string AuthToken { get; protected set; }

        public bool IsAnonymous { get; }
        protected ApiSession(Guid localIdentifier, string authToken)
        {
            IsAnonymous =string.IsNullOrEmpty(authToken);
            LocalIdentifier = localIdentifier;
            AuthToken = authToken;
        }

        public abstract void Dispose();

    }


}


