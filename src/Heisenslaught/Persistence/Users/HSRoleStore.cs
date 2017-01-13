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


        public HSRoleStore(IMongoDatabase database, ILoggerFactory loggerFactory):this(database, loggerFactory, "roles"){}

        public HSRoleStore(IMongoDatabase database, ILoggerFactory loggerFactory, string roleCollectionName)
        {
            _roleCollection = database.GetCollection<HSRole>(roleCollectionName);
            _logger = loggerFactory.CreateLogger(GetType().Name);
            EnsureIndiciesAsync();
        }
        private async void EnsureIndiciesAsync()
        {
            // TODO This should be configured in Startup.cs
            var role = await FindByNameAsync("SU", new CancellationToken());
            if(role == null)
            {
                role = new HSRole("su", "Super User");
                role.SetNameNormailzed("SU");
                await CreateAsync(role, new CancellationToken());
            }
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

        public async Task<IdentityResult> DeleteAsync(HSRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSRole>.Filter.Eq(r => r.Id, role.Id);
            await _roleCollection.DeleteOneAsync(query, null, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public void Dispose() {}

        public Task<HSRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if(roleId == null)
            {
                throw new ArgumentNullException(nameof(roleId));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSRole>.Filter.Eq(r => r.Id, roleId);
            return _roleCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<HSRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (normalizedRoleName == null)
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSRole>.Filter.Eq(r => r.NameNomalized, normalizedRoleName);
            return _roleCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(HSRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NameNomalized);
        }

        public Task<string> GetRoleIdAsync(HSRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NameNomalized);
        }

        public Task<string> GetRoleNameAsync(HSRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(HSRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            role.SetNameNormailzed(normalizedName);

            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(HSRole role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            role.SetName(roleName);

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(HSRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSRole>.Filter.Eq(r => r.Id, role.Id);
            var replaceResult = await _roleCollection.ReplaceOneAsync(query, role, new UpdateOptions { IsUpsert = false }).ConfigureAwait(false);
            return replaceResult.IsModifiedCountAvailable && replaceResult.ModifiedCount == 1
                ? IdentityResult.Success
                : IdentityResult.Failed();
        }
    }
}
