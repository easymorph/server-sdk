using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Dto.Auth



{
    [DataContract]
    internal class AuthProviderDto
    {

        /// <summary>
        /// Provider Id
        /// </summary>
        [DataMember(Name = "idPId")]
        public string IdPId { get; set; }
        /// <summary>
        /// Identity provider display name
        /// </summary>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
        /// <summary>
        /// Identity provider type
        /// </summary>
        [DataMember(Name = "idPType")]
        public string IdPType { get; set; }

        /// <summary>
        /// Support `Keep me signed in` option
        /// </summary>
        [DataMember(Name = "canKeepLongSession")]
        public bool CanKeepLongSession { get; set; }

    }
}
