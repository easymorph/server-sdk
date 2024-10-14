namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// Unknown or undefined. 
    /// </summary>
    public sealed class UnknownIdP: IdentityProviderBase
    {
        public UnknownIdP(string displayName, string idPId, string idPType) :base(displayName, Model.IdPType.Unknown, idPId)
        {

            IdPTypeRaw = idPType ?? throw new System.ArgumentNullException(nameof(idPType));
        }

        public string IdPTypeRaw { get; }
    }

}
