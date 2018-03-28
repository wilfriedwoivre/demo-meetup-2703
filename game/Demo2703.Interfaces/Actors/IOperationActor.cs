using Demo2703.Domain;
using Microsoft.ServiceFabric.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace Demo2703.Interfaces.Actors
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IOperationActor : IActor
    {
        Task<Operation> GetOperationAsync(CancellationToken cancellationToken);
        Task<Operation> GenerateOperationAsync(int difficulty, CancellationToken cancellationToken);

        Task<bool> IsValidResult(long result, CancellationToken cancellationToken);
    }
}