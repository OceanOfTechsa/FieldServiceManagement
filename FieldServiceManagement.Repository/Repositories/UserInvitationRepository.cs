using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Country;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using System.Linq.Expressions;

namespace FieldServiceManagement.Repository.Repositories
{
    public class UserInvitationRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<UserInvitation> _repository;
        private bool _disposed = false;

        public UserInvitationRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<UserInvitation>(_dbContext);
        }


        public UserInvitation GetInvitationByEmail(string Email)
        {
            return _repository.Find(x => x.Email == Email).FirstOrDefault()!;
        }
        public async void CreateUserInvitationAsync(UserInvitation model)
        {
            _repository.Insert(model);
        }

        public UserInvitation Find(Expression<Func<UserInvitation, bool>> predicate)
        {
            return _repository.Find(predicate)?.FirstOrDefault()!;
        }

        #region DISPOSE
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
                _disposed = true;
            }
        }

        ~UserInvitationRepository() => Dispose(false);
        #endregion
    }
}
