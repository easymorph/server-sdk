using Morph.Server.Sdk.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Morph.Server.Sdk.Exceptions;

namespace Morph.Server.Sdk.Model.SessionErrorHandling
{
    public class DefaultUnauthenticatedSessionErrorHandler : ISessionErrorHander
    {

        public DefaultUnauthenticatedSessionErrorHandler(IEnumerable<ISessionErrorHandlingStrategy> strategies)
        {
            if (strategies is null)
            {
                throw new ArgumentNullException(nameof(strategies));
            }
            Strategies = strategies;
        }

        public IEnumerable<ISessionErrorHandlingStrategy> Strategies { get; }


        HashSet<Guid> processingSessions = new HashSet<Guid>();
        object _lock = new object();
        public async Task<TResult> HandleAsync<TResult>(SessionErrorContext context, Func<ApiSession, Task<TResult>> retry, CancellationToken orginalCancellationToken)
        {
            try
            {
                
                if (context.OccuredException is MorphApiUnauthorizedException)
                {

                    // try to prevent infinite loop
                    lock (_lock)
                    {
                        if (processingSessions.Contains(context.ApiSession.LocalIdentifier))
                        {
                            throw context.OccuredException;
                        }
                        processingSessions.Add(context.ApiSession.LocalIdentifier);
                    }



                    foreach (var strategy in Strategies)
                    {
                        try
                        {
                            var result = await strategy.InvokeStrategyAsync(context, orginalCancellationToken);
                            switch (result)
                            {
                                case NotApplicable notApplicable:
                                    continue;
                                case RenewedSession renewedSession:
                                    return await retry(renewedSession.NewApiSession);

                            }
                        }
                        catch (Exception)
                        {
                            // nothing
                        }
                    }
                }

                throw context.OccuredException;
            }
            finally
            {
                processingSessions.Remove(context.ApiSession.LocalIdentifier);

            }

        }
    }

}