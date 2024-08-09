namespace Morph.Server.Sdk.Model
{

    public abstract class AuthenticatedUser
    {
    
    }

    public sealed class AnonumousAuthenticatedUser : AuthenticatedUser
    {
        public string AnonymousDueWrongSession { get; set; }
        
    }

    public sealed class LegacyAuthenticatedUser : AuthenticatedUser
    {
        public string SpaceName{ get; set; }
    }


    public sealed class RealAuthenticatedUser : AuthenticatedUser
    {

        /// <summary>
        /// UserID
        /// </summary>        
        public string UserId { get; internal set; }

        /// <summary>
        ///  User name
        /// </summary>        
        public string UserName { get; internal set; }


        /// <summary>
        ///  DisplayName
        /// </summary>        
        public string DisplayName { get; internal set; }


        /// <summary>
        /// Optional full name
        /// </summary>
        
        public string FullName { get; internal set; }

        /// <summary>
        /// Optional email
        /// </summary>        
        public string Email { get; internal set; }

        /// <summary>
        /// external Identity
        /// </summary>
        public string ExternalIdentity { get; internal set; }
    }
}