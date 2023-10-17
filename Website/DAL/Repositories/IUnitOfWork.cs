using StartupCentral.DAL.EntityModels;
using System;
using System.Collections;
using System.Threading.Tasks;
namespace StartupCentral.DAL.Repositories
{
    interface IUnitOfWork : IDisposable
    {
        IGenericRepository<userMessages> UserMessagRepository { get; }
        IGenericRepository<newsLetterSubscriptions> NewsLetterSubscriptionsRepository { get; }
        IGenericRepository<userForretningsPlan> ForretningsPlanRepository { get; }
        IGenericRepository<formularLogger> FormularLoggerRepository { get; }
        IGenericRepository<userPitch> PitchDeckRepository { get; }
        IGenericRepository<activeMembers> ActiveStudentsRepo { get; }
        IGenericRepository<activePlatformMembers> AllActiveMembersRepository { get; }
        void ExecuteSqlQuery(string query, params object[] parameters);
        IEnumerable ExecuteSqlReader(Type t, string query, params object[] parameters);
        void SetCommandTimeout(int timeout);
        void Save();
        Task SaveAsync();

    }
}