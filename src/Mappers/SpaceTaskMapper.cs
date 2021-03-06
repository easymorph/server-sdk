﻿using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Morph.Server.Sdk.Mappers
{

    internal static class RunningTaskStatusMapper
    {
        public static RunningTaskStatus RunningTaskStatusFromDto(RunningTaskStatusDto dto)
        {
            return new RunningTaskStatus
            {
                Id = Guid.Parse(dto.Id),
                IsRunning = dto.IsRunning,
                ProjectName = dto.ProjectName,                
                Errors = dto.Errors?.Select(SpaceTaskMapper.MapFromRunningTaskErrorInfoDto)?.ToList() ?? new List<ErrorInfo>()
            };
        }
    }

    internal static class SpaceTaskMapper
    {

        public static ErrorInfo MapFromRunningTaskErrorInfoDto(RunningTaskErrorInfoDto dto)
        {
            return new ErrorInfo
            {
                Description = dto.Description,
                Location = dto.Location
            };
        }
        public static SpaceTasksListItem MapItem(SpaceTasksListItemDto dto)
        {
            return new SpaceTasksListItem
            {
                Enabled = dto.Enabled,
                Id = Guid.Parse(dto.JobId),
                IsRunning = dto.IsRunning,
                TaskName = dto.Name,
                Note = dto.Note,
                ProjectPath = dto.ProjectFile,
                StatusText = dto.StatusText,
                TaskState = TaskStatusMapper.ParseTaskState(dto.Status)
            };
        }

        public static SpaceTask MapFull(SpaceTaskDto dto)
        {
            var rsult =  new SpaceTask
            {
                Enabled = dto.Enabled,
                Id = Guid.Parse(dto.JobId),
                IsRunning = dto.IsRunning,
                TaskName = dto.Name,
                Note = dto.Note,
                ProjectPath = dto.ProjectFile,
                StatusText = dto.StatusText,
                TaskState = TaskStatusMapper.ParseTaskState(dto.Status),
                
            };
            if(dto.JobParameters  != null)
            {
                rsult.TaskParameters = dto.JobParameters.Select(TaskParameterMapper.FromDto).ToList();
            }
            return rsult;            
        }
    }
}
