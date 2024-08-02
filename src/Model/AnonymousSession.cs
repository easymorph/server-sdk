namespace Morph.Server.Sdk.Model
{
    public sealed class AnonymousSession : ApiSession
    {
        internal AnonymousSession() : base(null)
        {
         
        }

        public override void Dispose()
        {
         // nothing to do
        }
    }


}


