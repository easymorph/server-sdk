using Morph.Server.Sdk.Client;
using System;
using System.Xml.Linq;

namespace Morph.Server.Sdk.Model
{

    public class ApiSessionFactory
    {
        public static AnonymousSession CreateAnonymousSession()
        {
            Guid localIdentifier = Guid.NewGuid();

            return new AnonymousSession(localIdentifier)
            {

            };

        }

        public static LegacyApiSession CreateLegacySession(ICanCloseSession canCloseSession, string spaceName, string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new System.ArgumentException($"'{nameof(authToken)}' cannot be null or whitespace.", nameof(authToken));
            }

            Guid localIdentifier = Guid.NewGuid();

            var legacySession = new LegacyApiSession(localIdentifier, spaceName, authToken);
            if (canCloseSession != null)
            {
                legacySession.SetClient(canCloseSession);
            }
            return legacySession;
        }

        public static PersitableApiSession RestorePersitableApiSession(Guid localIdentifier, string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new System.ArgumentException($"'{nameof(authToken)}' cannot be null or whitespace.", nameof(authToken));
            }

            return new PersitableApiSession(localIdentifier , authToken);
        }
        public static PersitableApiSession CreatePersitableApiSession(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new System.ArgumentException($"'{nameof(authToken)}' cannot be null or whitespace.", nameof(authToken));
            }

            Guid localIdentifier = Guid.NewGuid();

            return new PersitableApiSession(localIdentifier, authToken);
        }
    }


}


