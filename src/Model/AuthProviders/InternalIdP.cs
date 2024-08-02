namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// EasyMorph Internal identity provider (user-password)
    /// </summary>
    public sealed class InternalIdP : IdentityProviderBase,ISupportKeepingLongSession
    {
        public InternalIdP(string displayName, string idPId, bool canKeepLongSession) : base(displayName, IdPType.InternalIdP, idPId)
        {
            CanKeepLongSession = canKeepLongSession;
        }

        public bool CanKeepLongSession { get; }
    }

}
