using System;
using System.Collections.Generic;

namespace Morph.Server.Sdk.Model
{
    public class SpaceTasksListItem
    {
        public Guid Id { get; internal set; }
        public string TaskName { get; internal set; } = string.Empty;
        public string ProjectPath { get; internal set; } = string.Empty;
        public string Note { get; internal set; } = string.Empty;

        public TaskSchedule[] Schedules { get; internal set; } = Array.Empty<TaskSchedule>();        
        public bool Enabled { get; internal set; } = false;

        public string Group { get; internal set; } = string.Empty;


    }

    public sealed class SpaceTask : SpaceTasksListItem
    {
        public List<ParameterBase> TaskParameters { get; internal set; } = new List<ParameterBase>();
    }

    public sealed class TaskSchedule
    {
        public string ScheduleType { get; internal set; } = string.Empty;

        public string ScheduleDescription { get; internal set; } = String.Empty;
    }
}