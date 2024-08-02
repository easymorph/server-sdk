namespace Morph.Server.Sdk.Model
{

    public class AuthenticatedUser
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