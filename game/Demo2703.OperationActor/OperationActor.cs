using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Demo2703.Domain;
using Demo2703.Interfaces.Actors;

namespace Demo2703.OperationActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Volatile)]
    internal class OperationActor : Actor, IOperationActor, IRemindable
    {
        /// <summary>
        /// Initializes a new instance of OperationActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public OperationActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            var current = await GetOperationAsync(CancellationToken.None);
            if (current == null)
            {
                await this.StateManager.SetStateAsync<Operation>("operation", null);
                await this.StateManager.SetStateAsync<DateTime>("createdAt", DateTime.UtcNow);
                await this.StateManager.SetStateAsync<bool>("isActive", true);

                await this.RegisterReminderAsync("selfdestroy", null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            }
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            switch (reminderName)
            {
                case "selfdestroy":
                    var isActive = await this.StateManager.GetStateAsync<bool>("isActive");

                    if (!isActive)
                    {
                        IActorService serviceProxy = ActorServiceProxy.Create(this.ServiceUri, this.Id);
                        await serviceProxy.DeleteActorAsync(this.Id, CancellationToken.None);
                    }
                    break;
            }
        }

        public async Task<Operation> GetOperationAsync(CancellationToken cancellationToken)
        {
            var operation = await this.StateManager.TryGetStateAsync<Operation>("operation", cancellationToken);
            return operation.Value;
        }

        public async Task<Operation> GenerateOperationAsync(int difficulty, CancellationToken cancellationToken)
        {
            var operation = new Operation { Id = this.Id.GetGuidId() };

            Random rand = new Random();
            operation.NumberA = rand.Next((int)Math.Pow(10, difficulty + 1));
            operation.NumberB = rand.Next((int)Math.Pow(10, difficulty + 1));
            operation.Operand = (Operand)rand.Next((int)3);

            await this.StateManager.SetStateAsync<Operation>("operation", operation, cancellationToken);
            return operation;
        }

        public async Task<bool> IsValidResult(long result, CancellationToken cancellationToken)
        {
            var operation = await this.StateManager.TryGetStateAsync<Operation>("operation", cancellationToken);
            await this.StateManager.SetStateAsync<bool>("isActive", false, cancellationToken);

            if (operation.HasValue)
            {
                if (CalculateOperationResult(operation.Value) == result)
                {
                    return true;
                }
            }
            return false;
        }

        private long CalculateOperationResult(Operation operation)
        {
            switch (operation.Operand)
            {
                case Operand.Add:
                    return operation.NumberA + operation.NumberB;
                case Operand.Substract:
                    return operation.NumberA - operation.NumberB;
                case Operand.Multiply:
                    return operation.NumberA * operation.NumberB;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
