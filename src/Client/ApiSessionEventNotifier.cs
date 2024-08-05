using System;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Client
{
    public class ApiSessionEventNotifier : IApiSessionEventNotifier
    {
        public Func<ApiSessionUnauthenticatedEventData, Task> OnSessionUnathenticated { get; set; }
        public async Task InvokeOnApiSessionUnauthenticated(ApiSessionUnauthenticatedEventData eventData)
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            try
            {
                if (OnSessionUnathenticated != null)
                {
                    await OnSessionUnathenticated(eventData).ConfigureAwait(false);
                }
            }
            catch(Exception){
                // nothig
            }
        }
    }
}