namespace Morph.Server.Sdk.Model
{
    public sealed class OpenIdP : IdentityProviderBase, ISupportKeepingLongSession
    {
        public OpenIdP(string displayName, string idPId, bool canKeepLongSession) : base(displayName, IdPType.OpenId,idPId)
        {
            CanKeepLongSession = canKeepLongSession;
        }

        public bool CanKeepLongSession { get; }
    }

}
