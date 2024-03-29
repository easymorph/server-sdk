﻿using Morph.Server.Sdk.Dto.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Dto.Errors
{
    [DataContract]
    
    public class Error
    {

        /// <summary>
        ///  One of a server-defined set of error codes. (required)
        /// </summary>
        [DataMember]
        public string code { get; set; }

        /// <summary>
        /// A human-readable representation of the error. (required)
        /// </summary>
        [DataMember]
        public string message { get; set; }
        /// <summary>
        /// The target of the error.
        /// </summary>
        [DataMember]
        public string target { get; set; }
        /// <summary>
        /// An array of details about specific errors that led to this reported error.
        /// </summary>
        [DataMember]
        public List<Error> details { get; set; }
        [DataMember]
        public InnerError innererror { get; set; }

    }
}