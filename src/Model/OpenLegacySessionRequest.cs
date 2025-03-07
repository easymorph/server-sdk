﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model
{
    public class OpenLegacySessionRequest
    {
        public string SpaceName{ get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Create cloned request instance
        /// </summary>
        /// <returns>Memberwise clone</returns>
        public OpenLegacySessionRequest Clone() => (OpenLegacySessionRequest)MemberwiseClone();
    }
}