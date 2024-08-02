namespace Morph.Server.Sdk.Model
{
    /// <summary>
    ///  Anonymous provider
    /// </summary>
    public sealed class AnonymousIdP : IdentityProviderBase
    {
        public AnonymousIdP(string displayName, string idPId) : base(displayName, IdPType.Anonymous, idPId)
        {
        }
    }

}
