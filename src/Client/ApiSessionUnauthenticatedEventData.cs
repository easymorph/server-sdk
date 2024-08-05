using Morph.Server.Sdk.Model;

namespace Morph.Server.Sdk.Client
{
    public class ApiSessionUnauthenticatedEventData
    {
        public ApiSessionUnauthenticatedEventData(
            IMorphServerApiClient morphServerApiClient,ApiSession apiSession
            )
        {
            MorphServerApiClient = morphServerApiClient;
            ApiSession = apiSession;
        }

        public IMorphServerApiClient MorphServerApiClient { get; }
        public ApiSession ApiSession { get; }
    }
}