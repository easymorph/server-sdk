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

        
        public string AuthToken { get; protected set; }

        public bool IsAnonymous { get; }
        protected ApiSession(string authToken)
        {
            IsAnonymous =string.IsNullOrEmpty(authToken);
            
            AuthToken = authToken;
        }

        public abstract void Dispose();

        /// <summary>
        ///     Import authentication data from other token
        /// </summary>
        /// <param name="freshSession">Session to import from</param>
        /// <exception cref="ArgumentNullException"><see cref="freshSession"/> is null</exception>
        public void FillFrom(ApiSession freshSession)
        {
            if (freshSession == null) throw new ArgumentNullException(nameof(freshSession));

            AuthToken = freshSession.AuthToken;
        }
    }


}


