namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// ActiveDirectory Seamless login
    /// </summary>
    public sealed class AdSeamlessIdP : IdentityProviderBase, ISupportKeepingLongSession
    {
        public AdSeamlessIdP(string displayName, string idPId, bool canKeepLongSession) : base(displayName, IdPType.AdSeamlessIdP, idPId)
        {
            CanKeepLongSession = canKeepLongSession;
        }

        public bool CanKeepLongSession { get; }
    }

}
