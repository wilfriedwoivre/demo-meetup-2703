using Demo2703.Domain;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo2703.Interfaces.Services
{
    public interface ISessionManager : IService
    {
        Task CreateSessionAsync(Session session);
        Task<Session> GetSessionAsync(Guid id);
        Task AddGameToSessionAsync(Guid idSession, Operation operation, bool success);
        Task<List<Session>> ListSessionAsync(); 
    }
}
