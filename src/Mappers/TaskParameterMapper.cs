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
        public static ParameterBase FromDto(TaskParameterResponseDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var parameterType = ParseParameterType(dto.ParameterType);
            switch (parameterType)
            {
                case ParameterType.Text: 
                    return new TextParameter(dto.Name, dto.Value) { Note = dto.Note };

                case ParameterType.Date:
                    return new DateParameter(dto.Name, dto.Value) { Note = dto.Note };

                case ParameterType.FilePath:
                    return new FilePathParameter(dto.Name, dto.Value) { Note = dto.Note };

                case ParameterType.FolderPath:
                    return new FolderPathParameter(dto.Name, dto.Value) { Note = dto.Note };

                case ParameterType.Checkbox:
                    return new CheckboxParameter(dto.Name, dto.Value) { Note = dto.Note };

                case ParameterType.FixedList:
                    return new FixedListParameter(dto.Name, dto.Value, MapAvailableValues(dto.Details?.AvailableValues)) { Note = dto.Note };

                case ParameterType.MultipleChoice:
                    return new MultipleChoiceParameter(dto.Name, dto.Value, dto.Details?.SepatatorString, MapAvailableValues(dto.Details?.AvailableValues)) { Note = dto.Note };


                default:
                    return new TextParameter(dto.Name, dto.Value) { Note = dto.Note };
            }

           
        }

        private static MorphParameterValueListItem[] MapAvailableValues(MorphParameterValueListItemDto[] availableValues)
        {
            if (availableValues == null)
                return new MorphParameterValueListItem[] { };

            return availableValues.Select(x => new MorphParameterValueListItem(x.Label, x.Value)).ToArray();
        }

        private static ParameterType ParseParameterType(string value)
        {
            //fallback to text
            if (string.IsNullOrWhiteSpace(value))
                return ParameterType.Text;

            return (ParameterType)Enum.Parse(typeof(ParameterType), value, true);
        }

        public static TaskParameterRequestDto ToDto(ParameterBase value)
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
        public static TaskParameterRequestDto ToDto(ParameterNameValue parameterNameValue)
        {
            if (parameterNameValue == null)
            {
                throw new ArgumentNullException(nameof(parameterNameValue));
            }

            var result = new TaskParameterRequestDto()
            {
                Name = parameterNameValue.Name,
                Value = parameterNameValue.Value

            };


            return result;

        }



    }

}
