using Morph.Server.Sdk.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model.SessionErrorHandling
{
    public interface ISessionErrorHander
    {
        Task<TResult> HandleAsync<TResult>(SessionErrorContext context, Func<ApiSession, Task<TResult>> value, CancellationToken orginalCancellationToken);
    }

}