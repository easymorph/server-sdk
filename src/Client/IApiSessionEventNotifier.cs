using System;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Client
{
    public interface IApiSessionEventNotifier
    {
        Func<ApiSessionUnauthenticatedEventData,Task> OnSessionUnathenticated { get; set; }
        Task InvokeOnApiSessionUnauthenticated(ApiSessionUnauthenticatedEventData eventData);
    }
}