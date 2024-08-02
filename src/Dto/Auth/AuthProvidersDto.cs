using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Dto.Auth

{
    [DataContract]
    internal class AuthProvidersDto
    {

        /// <summary>
        /// Provider's list
        /// </summary>
        [DataMember(Name = "values")]
        public AuthProviderDto[] Values { get; set; }        

    }
}
