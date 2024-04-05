using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model
{


    public class ParameterBase
    {   

        public string Name { get; } = string.Empty;
        public string Value { get; protected set; } = string.Empty;
        public TaskParameterType ParameterType { get; } 
        public string Note { get; set; }

        public ParameterBase(TaskParameterType parameterType, string name)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter name is empty", nameof(name));
            }
            ParameterType = parameterType;
            this.Name = name;
        }

        public virtual ParameterNameValue GetParameterNameValue()
        {
            return new ParameterNameValue(Name, Value);
        }

    }


    public sealed class TextParameter : ParameterBase
    {     
        public TextParameter(string name, string value) : base(TaskParameterType.Text, name)
        {
            this.Value = value ?? string.Empty;
        }
    }

    public sealed class FilePathParameter : ParameterBase
    {
        
        public FilePathParameter(string name,  string value) : base(TaskParameterType.FilePath, name)
        {
            this.Value = value ?? string.Empty;
        }
    }
    public sealed class FolderPathParameter : ParameterBase
    {        
        public FolderPathParameter(string name, string value) : base(TaskParameterType.FolderPath, name)
        {            
            this.Value = value ?? string.Empty;            
        }
    }

    public sealed class DateParameter : ParameterBase
    {
        /// <summary>
        /// Nullable
        /// </summary>
        public DateOnly DateValue { get; private set; }
        public DateParameter(string name, DateOnly dateValue) : base(TaskParameterType.Date, name)
        {

            DateValue = dateValue;
            var isoDateValue = dateValue?.ToIsoDate() ?? string.Empty;
            this.Value = isoDateValue;
        }

        public DateParameter(string name, string orignalValue) : base(TaskParameterType.Date, name)
        {

            DateValue = DateOnly.FromIsoDate(orignalValue);            
            this.Value = orignalValue ?? string.Empty;
        }
    }

    public sealed class CheckboxParameter : ParameterBase
    {

        private static bool IsChecked(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return new string[] { "Y", "YES", "1", "T", "TRUE" }.Contains(value.ToUpperInvariant());

        }
        public bool Checked { get; private set; }
        public CheckboxParameter(string name, string orignalValue) : base(TaskParameterType.Checkbox, name)
        {
            this.Value = orignalValue ?? string.Empty;
            this.Checked = IsChecked(this.Value);

        }
        public CheckboxParameter(string name, bool @checked) : base(TaskParameterType.Checkbox, name)
        {
            this.Value = @checked ? "true" : "false";
            this.Checked = @checked;
        }


    }


    public sealed class MorphParameterValueListItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public MorphParameterValueListItem(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }

    public sealed class FixedListParameter : ParameterBase {
        public MorphParameterValueListItem[] AvailableValues { get; }
        public FixedListParameter(string name, string value, MorphParameterValueListItem[] availableValues) 
            : base(TaskParameterType.FixedList, name)
        {
            this.Value = value ?? string.Empty;
            AvailableValues = availableValues ?? new MorphParameterValueListItem[] { };
        }        
    }

    public sealed class MultipleChoiceParameter : ParameterBase
    {
        public MorphParameterValueListItem[] AvailableValues { get; }
        public string SeparatorString { get;  }
        public string[] SelectedValues { get; }

        public MultipleChoiceParameter(string name, string value, string separatorString,   MorphParameterValueListItem[] availableValues)
            : base(TaskParameterType.MultipleChoice, name)
        {
            if (string.IsNullOrEmpty(separatorString))
            {
                throw new ArgumentNullException(nameof(separatorString));
            }

            this.SeparatorString = separatorString;
            this.Value = value ?? string.Empty;
            this.SelectedValues  = (value ?? string.Empty).Split(new[] { separatorString }, StringSplitOptions.RemoveEmptyEntries);
            
            AvailableValues = availableValues ?? new MorphParameterValueListItem[] {};
        }
    }
}
