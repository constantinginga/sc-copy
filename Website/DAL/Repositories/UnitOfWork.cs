using StartupCentral.DAL.EntityModels;
using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StartupCentral.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private bool _onlyReadData;
        protected DbContext _context;

        private IGenericRepository<userMessages> _UserMessagRepository;
        private IGenericRepository<newsLetterSubscriptions> _NewsLetterSubscriptions;
        private IGenericRepository<userForretningsPlan> _UserForretningsPlan;
        private IGenericRepository<formularLogger> _FormularLogger;
        private IGenericRepository<userPitch> _UserPitch;
        private IGenericRepository<activeMembers> _ActiveMembers;
        private IGenericRepository<activePlatformMembers> _ActivePlatformMembers;

        public UnitOfWork()
        {
            _context = new startupcentraldkEntities();
            _disposed = false;
            _onlyReadData = false;
    }

        public IGenericRepository<userMessages> UserMessagRepository
        {
            get { return _UserMessagRepository ?? (_UserMessagRepository = new GenericRepository<userMessages>(_context)); }
        }

        public IGenericRepository<newsLetterSubscriptions> NewsLetterSubscriptionsRepository { 
            get { return _NewsLetterSubscriptions ?? (_NewsLetterSubscriptions = new GenericRepository<newsLetterSubscriptions>(_context)); }
        }

        public IGenericRepository<userForretningsPlan> ForretningsPlanRepository 
        {
            get { return _UserForretningsPlan ?? (_UserForretningsPlan = new GenericRepository<userForretningsPlan>(_context)); }
        }

        public IGenericRepository<formularLogger> FormularLoggerRepository
        {
            get { return _FormularLogger ?? (_FormularLogger = new GenericRepository<formularLogger>(_context)); }
        }

        public IGenericRepository<userPitch> PitchDeckRepository
        {
            get { return _UserPitch ?? (_UserPitch = new GenericRepository<userPitch>(_context)); }
        }

        public IGenericRepository<activeMembers> ActiveStudentsRepo
        {
            get { return _ActiveMembers ?? (_ActiveMembers = new GenericRepository<activeMembers>(_context)); }
        }

        public IGenericRepository<activePlatformMembers> AllActiveMembersRepository
        {
            get { return _ActivePlatformMembers ?? (_ActivePlatformMembers = new GenericRepository<activePlatformMembers>(_context)); }
        }

        public void Save()
        {
            if (_onlyReadData)
                throw new Exception();

            try
            {
                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                var outputLines = new System.Text.StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.AppendLine(string.Format(
                        "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                        DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.AppendLine(string.Format(
                            "- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw new Exception(outputLines.ToString());
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void ExecuteSqlQuery(string query, params object[] parameters)
        {
            _context.Database.ExecuteSqlCommand(query, parameters);
        }

        private void ExecuteQuery(string query, string connectionString)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.CommandTimeout = 5 * 60; 

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                }
                reader.Close();
            }
        }

        public IEnumerable ExecuteSqlReader(Type t, string query, params object[] parameters)
        {
            return _context.Database.SqlQuery(t, query, parameters);
        }

        public void SetCommandTimeout(int timeout)
        {
            ((IObjectContextAdapter)_context).ObjectContext.CommandTimeout = 180;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}