using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo2703.Domain;
using Demo2703.Interfaces.Services;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Demo2703.SessionManager
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SessionManager : StatefulService, ISessionManager
    {
        public SessionManager(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task CreateSessionAsync(Session session)
        {
            using (var txn = this.StateManager.CreateTransaction())
            {
                var sessions = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Session>>("sessions");
                await sessions.GetOrAddAsync(txn, session.Id, session);
                await txn.CommitAsync();
            }
        }

        public async Task<Session> GetSessionAsync(Guid id)
        {
            using (var txn = this.StateManager.CreateTransaction())
            {
                var sessions = await this.StateManager.TryGetAsync<IReliableDictionary<Guid, Session>>("sessions");
                if (!sessions.HasValue)
                    throw new KeyNotFoundException($"Session {id} not found");
                var conditionalValue = await sessions.Value.TryGetValueAsync(txn, id);
                if (!conditionalValue.HasValue)
                    throw new KeyNotFoundException($"Session {id} not found");

                return conditionalValue.Value;
            }
        }

        public async Task AddGameToSessionAsync(Guid idSession, Operation operation, bool success)
        {
            var session = await GetSessionAsync(idSession);
            session.GameCount++;
            session.TotalScore += success ? Math.Abs(CalculateOperationResult(operation)) : -Math.Abs(CalculateOperationResult(operation));

            using (var txn = this.StateManager.CreateTransaction())
            {
                var sessions = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Session>>("sessions");
                await sessions.SetAsync(txn, session.Id, session);
                await txn.CommitAsync();
            }

        }

        public async Task<List<Session>> ListSessionAsync()
        {
            using (var txn = this.StateManager.CreateTransaction())
            {
                var result = new List<Session>();
                var categories = await this.StateManager.TryGetAsync<IReliableDictionary<Guid, Session>>("sessions");
                if (categories.HasValue)
                {
                    var enumerable = await categories.Value.CreateEnumerableAsync(txn, EnumerationMode.Unordered);
                    var enumerator = enumerable.GetAsyncEnumerator();
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(enumerator.Current.Value);
                    }
                }
                return result;
            }
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
