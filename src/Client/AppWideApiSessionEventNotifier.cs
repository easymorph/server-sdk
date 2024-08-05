using System;
using System.Threading;

namespace Morph.Server.Sdk.Client
{
    public static class AppWideApiSessionEventNotifier
    {
        private static readonly Lazy<ApiSessionEventNotifier> Provider = new Lazy<ApiSessionEventNotifier>(
            () => new ApiSessionEventNotifier(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static IApiSessionEventNotifier Instance => Provider.Value;
    }
}