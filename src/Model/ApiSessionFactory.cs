using Morph.Server.Sdk.Client;

namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// Disposable api session
    /// </summary>
    /// 



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
            return new LegacyApiSession(canCloseSession, spaceName,  authToken);
        }

        public static PersitableApiSession CreatePersitableApiSession(string authToken)
        {
            return new PersitableApiSession(authToken);
        }
    }


}


