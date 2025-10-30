namespace Morph.Server.Sdk.Model
{
    public sealed class EntraIdP : IdentityProviderBase, ISupportKeepingLongSession
    {
        public EntraIdP(string displayName, string idPId, bool canKeepLongSession) : base(displayName, IdPType.EntraIdP, idPId)
        {
            CanKeepLongSession = canKeepLongSession;
        }

        public bool CanKeepLongSession { get; }
    }
}
