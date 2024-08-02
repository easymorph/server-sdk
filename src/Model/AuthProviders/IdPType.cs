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

}
