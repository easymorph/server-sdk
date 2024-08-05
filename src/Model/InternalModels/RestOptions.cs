using System.Collections.Generic;

namespace Morph.Server.Sdk.Model.InternalModels
{
    public static class RestOptionsNames
    {
        public const string Session = "session";
        public const string Client = "client";
    }

    public class RestOptions
    {
        private Dictionary<string, object> _options = new Dictionary<string, object>();

        public void Set(string key, object value)
        {
            _options[key] = value;
        }
        public bool TryGet<T>(string key, out T value)
        {
            if (_options.TryGetValue(key, out object v) && v is T casted)
            {
                value = casted;
                return true;
            }
            value = default;
            return false;
        }
    }


}