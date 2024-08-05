using System;
using System.Threading;

namespace Morph.Server.Sdk.Client
{
    /// <summary>
    ///     Provides one-per-AppDomain session refresher to be used as sensible default/fallback refresher
    /// </summary>
    internal static class AppWideLegacyApiSessionRefresher
    {
        private static readonly Lazy<LegacyApiSessionRefresher> Provider = new Lazy<LegacyApiSessionRefresher>(
            () => new LegacyApiSessionRefresher(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static LegacyApiSessionRefresher Instance => Provider.Value;
    }
}