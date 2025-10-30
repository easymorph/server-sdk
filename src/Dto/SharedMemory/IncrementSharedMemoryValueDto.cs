using System.Runtime.Serialization;

namespace Morph.Server.Sdk.Dto.SharedMemory
{
    [DataContract]
    internal class IncrementSharedMemoryValueDto
    {
        public static class BehaviorCodes
        {
            public const string Throw = "Throw";
            public const string UseDefault = "UseDefault";
        }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "value")]
        public decimal Value { get; set; }

        [DataMember(Name = "missingKeyBehavior")]
        public string MissingKeyBehavior { get; set; }
    }
}
