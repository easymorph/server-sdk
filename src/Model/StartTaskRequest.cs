﻿using System;
using System.Collections.Generic;

namespace Morph.Server.Sdk.Model
{
    public sealed class StartTaskRequest
    {
        public Guid TaskId { get; set; }
        
        public string UploadContextId { get; set; }

        public IEnumerable<ParameterNameValue> TaskParameters { get; set; }

        public StartTaskRequest(Guid taskId)
        {
            TaskId = taskId;
        }
        
    }
}