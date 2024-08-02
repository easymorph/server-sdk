namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// PersistableApiSession might be stored locally and reused
    /// </summary>
    public class PersitableApiSession : ApiSession
    {
        internal PersitableApiSession(string authToken) : base(authToken)
        {
        }

        public override void Dispose()
        {
            // nothing to do
        }

        // TODO: serialization/deserialization
    }


}


