using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model
{
    public class ParameterNameValue
    {
        public string Name { get; } = string.Empty;
        public string Value { get; } = string.Empty;

        public ParameterNameValue(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter name is empty", nameof(name));
            }

            Name = name;
            Value = value ?? string.Empty;
        }
    }
}
