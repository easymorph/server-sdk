using Morph.Server.Sdk.Dto;
using Morph.Server.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Mappers
{
    

    internal static class TaskParameterMapper
    {
        public static TaskParameterBase FromDto(TaskParameterResponseDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var parameterType = ParseParameterType(dto.ParameterType);
            switch (parameterType)
            {
                case TaskParameterType.Text: 
                    return new TaskTextParameter(dto.Name, dto.Value) { Note = dto.Note };

                case TaskParameterType.Date:
                    return new TaskDateParameter(dto.Name, dto.Value) { Note = dto.Note };

                case TaskParameterType.FilePath:
                    return new TaskFilePathParameter(dto.Name, dto.Value) { Note = dto.Note };

                case TaskParameterType.FolderPath:
                    return new TaskFolderPathParameter(dto.Name, dto.Value) { Note = dto.Note };

                case TaskParameterType.Checkbox:
                    return new TaskCheckboxParameter(dto.Name, dto.Value) { Note = dto.Note };

                case TaskParameterType.FixedList:
                    return new TaskFixedListParameter(dto.Name, dto.Value, MapAvailableValues(dto.Details?.AvailableValues)) { Note = dto.Note };

                case TaskParameterType.MultipleChoice:
                    return new TaskMultipleChoiceParameter(dto.Name, dto.Value, dto.Details?.SepatatorString, MapAvailableValues(dto.Details?.AvailableValues)) { Note = dto.Note };


                default:
                    return new TaskTextParameter(dto.Name, dto.Value) { Note = dto.Note };
            }

           
        }

        private static MorphParameterValueListItem[] MapAvailableValues(MorphParameterValueListItemDto[] availableValues)
        {
            if (availableValues == null)
                return new MorphParameterValueListItem[] { };

            return availableValues.Select(x => new MorphParameterValueListItem(x.Label, x.Value)).ToArray();
        }

        private static TaskParameterType ParseParameterType(string value)
        {
            //fallback to text
            if (string.IsNullOrWhiteSpace(value))
                return TaskParameterType.Text;

            return (TaskParameterType)Enum.Parse(typeof(TaskParameterType), value, true);
        }

        public static TaskParameterRequestDto ToDto(TaskParameterBase value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var result = new TaskParameterRequestDto()
            {
                Name = value.Name,
                Value = value.Value               
                
            };


            return result;
            
        }

       

    }
}
