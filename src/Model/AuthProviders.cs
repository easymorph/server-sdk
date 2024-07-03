using Morph.Server.Sdk.Mappers;
using System.Collections.Generic;

namespace Morph.Server.Sdk.Model
{



    /// <summary>
    /// Identity provider type.    
    /// </summary>
    public enum IdPType
    {
        /// <summary>
        /// Unknown or undefined. 
        /// </summary>
        Unknown,
        /// <summary>
        /// Anonymous provider
        /// </summary>
        Anonymous,
        /// <summary>
        /// Space password
        /// </summary>
        SpacePwd,
        /// <summary>
        /// EasyMorph Internal identity provider (user-password)
        /// </summary>
        InternalIdP,
        /// <summary>
        /// ActiveDirectory Seamless login
        /// </summary>
        AdSeamlessIdP,
        /// <summary>
        /// Rescue login (Privileged Windows user via Server Monitor)
        /// </summary>
        RescueLogin,

        OpenId
       
    }


    public abstract class IdentityProviderBase
    {
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
        protected IdentityProviderBase(IdPType idPType, string idPId)
        {
            IdPType = idPType;
            IdPId = idPId;
        }
    }
    /// <summary>
    /// Unknown or undefined. 
    /// </summary>
    public sealed class UnknownIdP: IdentityProviderBase
    {
        public UnknownIdP(string idPId, string idPType) :base(Model.IdPType.Unknown, idPId)
        {

            IdPType = idPType ?? throw new System.ArgumentNullException(nameof(idPType));
        }

        public string IdPType { get; }
    }

    /// <summary>
    ///  Anonymous provider
    /// </summary>
    public sealed class AnonymousIdP : IdentityProviderBase
    {
        public AnonymousIdP(string idPId) : base(IdPType.Anonymous, idPId)
        {
        }
    }


    /// <summary>
    /// Space password
    /// </summary>
    /// 
    public sealed class SpacePwdIdP : IdentityProviderBase
    {
        public SpacePwdIdP(string idPId) : base(IdPType.SpacePwd,idPId)
        {
        }
    }

    /// <summary>
    /// EasyMorph Internal identity provider (user-password)
    /// </summary>
    public sealed class InternalIdP : IdentityProviderBase
    {
        public InternalIdP(string idPId) : base(IdPType.InternalIdP, idPId)
        {
        }
    }


    /// <summary>
    /// ActiveDirectory Seamless login
    /// </summary>
    public sealed class AdSeamlessIdP : IdentityProviderBase
    {
        public AdSeamlessIdP(string idPId) : base(IdPType.AdSeamlessIdP, idPId)
        {
        }
    }

    public sealed class OpenIdP : IdentityProviderBase
    {
        public OpenIdP(string idPId) : base(IdPType.OpenId,idPId)
        {
        }
    }

    /// <summary>
    /// Rescue login (Privileged Windows user via Server Monitor)
    /// </summary>
    public sealed class RescueLoginIdP : IdentityProviderBase
    {
        public RescueLoginIdP(string idPId) : base(IdPType.RescueLogin,idPId)
        {
        }
    }



    public class AuthProvider
    {
        
        /// <summary>
        /// Identity provider display name
        /// </summary>
        public string DisplayName { get; internal set; }
        /// <summary>
        /// Identity provider type
        /// </summary>
        public IdentityProviderBase IdentityProvider { get; internal set; }

        /// <summary>
        /// Support `Keep me signed in` option
        /// </summary>
        public bool CanKeepLongSession { get; internal set; }
    }


    public class AuthProvidersList
    {
         public List<AuthProvider> Items { get; internal set; }
    }

}
