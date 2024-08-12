using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model.SessionErrorHandling
{
    public interface ISessionErrorHandlingStrategy
    {
        Task<SessionErrorHandlingStrategyResult> InvokeStrategyAsync(SessionErrorContext context, CancellationToken orginalCancellationToken);
    }

}