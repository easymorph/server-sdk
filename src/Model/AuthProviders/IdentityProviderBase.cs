namespace Morph.Server.Sdk.Model
{
    public abstract class IdentityProviderBase
    {


        /// <summary>
        /// Identity provider display name
        /// </summary>
        public virtual string DisplayName { get; protected set; }
        
        /// <summary>
        /// Provider Id
        /// </summary>
        public virtual string IdPId { get; protected set; }
        /// <summary>
        /// Provider type
        /// </summary>
        public virtual IdPType IdPType { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPType"></param>
        /// <param name="idPId"></param>
        protected IdentityProviderBase(string displayName, IdPType idPType, string idPId)
        {
            DisplayName = displayName;
            IdPType = idPType;
            IdPId = idPId;
        }
    }

}
