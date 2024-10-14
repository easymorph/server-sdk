namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// Space password
    /// </summary>
    /// 
    public sealed class SpacePwdIdP : IdentityProviderBase
    {
        public SpacePwdIdP(string displayName, string idPId) : base(displayName, IdPType.SpacePwd,idPId)
        {
        }
    }

}
