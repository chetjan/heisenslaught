using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace Heisenslaught.Persistence
{
    public class HSRoleRepository<TRole> : IRoleStore<TRole> where TRole : IdentityRole
    {
        private readonly IMongoCollection<TRole> _roleCollection;

        public HSRoleRepository(IMongoDatabase database, ILoggerFactory loggerFactory) 
        {
            _roleCollection = database.GetCollection<TRole>("roles");
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _roleCollection.InsertOneAsync(role, null, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;   
         }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<TRole>.Filter.Eq(r => r.Id, role.Id);
            await _roleCollection.DeleteOneAsync(query, null, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public void Dispose() {}

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
