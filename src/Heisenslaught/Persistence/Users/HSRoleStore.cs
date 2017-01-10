using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
using System.Threading;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace Heisenslaught.Persistence.User
{
    public class HSRoleStore : IRoleStore<HSRole>
    {

        private readonly IMongoCollection<HSRole> _roleCollection;
        private readonly ILogger _logger;

        public HSRoleStore(IMongoDatabase database, ILoggerFactory loggerFactory, string collectionName)
        {
            _roleCollection = database.GetCollection<HSRole>(collectionName);
            _logger = loggerFactory.CreateLogger(GetType().Name);
            EnsureIndicies();
        }
        private void EnsureIndicies()
        {

        }

        public async Task<IdentityResult> CreateAsync(HSRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            cancellationToken.ThrowIfCancellationRequested();
            await _roleCollection.InsertOneAsync(role, null, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(HSRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<HSRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HSRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(HSRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(HSRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(HSRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(HSRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(HSRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(HSRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
