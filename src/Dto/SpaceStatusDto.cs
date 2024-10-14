﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Dto
{
    [DataContract]
    internal class SpaceStatusDto
    {
        [DataMember(Name = "spaceName")]
        public string SpaceName { get; set; }
        [DataMember(Name = "isPublic")]
        public bool IsPublic { get; set; }
        [DataMember(Name = "userPermissions")]
        public List<string> UserPermissions { get; set; }

        [DataMember (Name = "spaceDescription")]
        public string SpaceDescription { get; set; }


    }
}
