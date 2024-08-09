using System.Runtime.Serialization;

namespace Morph.Server.Sdk.Dto
{

    



    [DataContract]
    internal class AuthenticatedUserDto
    {

        [DataMember(Name = "realUser")] 
        public RealAuthenticatedUserDto RealUser { get; set; } 
        [DataMember(Name = "legacyUser")] 
        public LegacyAuthenticatedUserDto LegacyUser { get; set; } 
        [DataMember(Name = "anonymous")]         
        public AnonymousAuthenticatedUserDto Anonymous { get; set; } 


    }



    [DataContract]
    internal class AnonymousAuthenticatedUserDto
    {

        
        [DataMember(Name = "anonymousDueWrongSession")]
        public string AnonymousDueWrongSession { get; set; }



    }


    [DataContract]
    internal class LegacyAuthenticatedUserDto
    {

        /// <summary>
        /// spaceName
        /// </summary>
        [DataMember(Name = "spaceName")]
        public string SpaceName { get; set; }



    }

    [DataContract]
    internal class RealAuthenticatedUserDto
    {

        /// <summary>
        /// UserID
        /// </summary>
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        /// <summary>
        ///  User login
        /// </summary>
        [DataMember(Name = "userName")]
        public string UserName { get; set; }


        /// <summary>
        ///  DisplayName
        /// </summary>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }


        /// <summary>
        /// Optional user name
        /// </summary>
        [DataMember(Name = "fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// Optional email
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// external Identity
        /// </summary>
        [DataMember(Name = "externalIdentity")]
        public string ExternalIdentity { get; set; }


    }
}
