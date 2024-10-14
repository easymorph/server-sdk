namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// Rescue login (Privileged Windows user via Server Monitor)
    /// </summary>
    public sealed class RescueLoginIdP : IdentityProviderBase
    {
        public RescueLoginIdP(string displayName, string idPId) : base(displayName, IdPType.RescueLogin,idPId)
        {
        }
    }

}
