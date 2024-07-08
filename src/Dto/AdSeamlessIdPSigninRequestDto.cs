using System.Runtime.Serialization;

namespace Morph.Server.Sdk.Dto
{
    [DataContract]
    internal class AdSeamlessIdPSigninRequestDto
    {
        [DataMember(Name = "requestToken")]
        public string RequestToken { get; set; }        
    }
}
