using System;

namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// Marks that session can be serialized
    /// </summary>
    public interface ISerializableApiSession
    {
        Guid LocalIdentifier { get; }
        string AuthToken { get;  }
    }


}


