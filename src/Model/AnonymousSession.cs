using System;

namespace Morph.Server.Sdk.Model
{
    public sealed class AnonymousSession : ApiSession
    {
        internal AnonymousSession(Guid localIdentifier) : base(localIdentifier, null)
        {
         
        }

        public override void Dispose()
        {
         // nothing to do
        }
    }


}


