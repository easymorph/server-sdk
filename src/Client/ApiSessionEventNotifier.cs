using System;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Client
{
    public class ApiSessionEventNotifier : IApiSessionEventNotifier
    {
        public Func<ApiSessionUnauthenticatedEventData, Task> OnSessionUnauthenticated { get; set; }
        public async Task InvokeOnApiSessionUnauthenticated(ApiSessionUnauthenticatedEventData eventData)
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            try
            {
                if (OnSessionUnauthenticated != null)
                {
                    await OnSessionUnauthenticated(eventData).ConfigureAwait(false);
                }
            }
            catch(Exception){
                // nothig
            }
        }
    }
}