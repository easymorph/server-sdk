using System;

namespace Morph.Server.Sdk.Model
{

    /// <summary>
    /// PersistableApiSession might be stored locally and reused
    /// </summary>
    /// 
    public class PersitableApiSession : ApiSession, ISerializableApiSession
    {
        internal PersitableApiSession(Guid localIdentifier, string authToken) : base(localIdentifier, authToken)
        {
        }

        public override void Dispose()
        {
            // nothing to do
        }

    }


}


