using Morph.Server.Sdk.Client;
using System;
using System.Xml.Linq;

namespace Morph.Server.Sdk.Model
{

    public class ApiSessionFactory
    {
        public static AnonymousSession CreateAnonymousSession()
        {
            return new AnonymousSession()
            {

            };

        }

        public static LegacyApiSession CreateLegacySession(ICanCloseSession canCloseSession, string spaceName, string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new System.ArgumentException($"'{nameof(authToken)}' cannot be null or whitespace.", nameof(authToken));
            }

            return new LegacyApiSession(canCloseSession, spaceName, authToken);
        }

        public static PersitableApiSession CreatePersitableApiSession(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new System.ArgumentException($"'{nameof(authToken)}' cannot be null or whitespace.", nameof(authToken));
            }

            return new PersitableApiSession(authToken);
        }
    }


}


