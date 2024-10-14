using Morph.Server.Sdk.Client;
using Morph.Server.Sdk.Model.InternalModels;
using System;

namespace Morph.Server.Sdk.Model.SessionErrorHandling
{
    /// <summary>
    /// d
    /// </summary>
    public class SessionErrorContext
    {
        public SessionErrorContext(IMorphServerApiClient morphServerApiClient, ApiSession apiSession, OperationType operationType, Exception occuredException)
        {
            MorphServerApiClient = morphServerApiClient ?? throw new ArgumentNullException(nameof(morphServerApiClient));
            ApiSession = apiSession ?? throw new ArgumentNullException(nameof(apiSession));
            OperationType = operationType;
            OccuredException = occuredException ?? throw new ArgumentNullException(nameof(occuredException));
        }

        public IMorphServerApiClient MorphServerApiClient { get; }
        public ApiSession ApiSession { get; }
        public OperationType OperationType { get; }
        public Exception OccuredException { get; }
    }

}
