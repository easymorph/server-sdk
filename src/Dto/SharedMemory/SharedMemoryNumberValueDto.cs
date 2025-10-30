using System.Runtime.Serialization;

namespace Morph.Server.Sdk.Dto.SharedMemory
{
    [DataContract]
    internal class SharedMemoryNumberValueDto
    {
        [DataMember(Name = "value")]
        public decimal? Value { get; set; }
    }
}
