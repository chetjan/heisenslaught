using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
using System.Threading;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

namespace Heisenslaught.Persistence.User
{
    public class HSUserStore : IUserStore<HSUser>, IUserLoginStore<HSUser>, IUserPasswordStore<HSUser>, IUserEmailStore<HSUser>
    {
        private readonly IMongoCollection<HSUser> _userCollection;
        private readonly ILogger _logger;

        public HSUserStore(IMongoDatabase database, ILoggerFactory loggerFactory, string collectionName)
        {
            _userCollection = database.GetCollection<HSUser>(collectionName);
            _logger = loggerFactory.CreateLogger(GetType().Name);
            EnsureIndicies();
        }
        
        public async Task<IdentityResult> CreateAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            cancellationToken.ThrowIfCancellationRequested();
            await _userCollection.InsertOneAsync(user, null, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;

        }

        public async Task<IdentityResult> DeleteAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSUser>.Filter.Eq(u => u.Id, user.Id);
            await _userCollection.DeleteOneAsync(query, null, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public Task<HSUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSUser>.Filter.Eq(u => u.Id, userId);
            return _userCollection.Find(query).FirstOrDefaultAsync(cancellationToken);

        }

        public Task<HSUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (normalizedUserName == null)
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }
            cancellationToken.ThrowIfCancellationRequested();
            var query = Builders<HSUser>.Filter.Eq(u => u.BattleTagNormaized, normalizedUserName);
            return _userCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.BattleTagNormaized);
        }

        public Task<string> GetUserIdAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.BattleTag);
        }

        public Task SetNormalizedUserNameAsync(HSUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            user.BattleTagNormaized = normalizedName;

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(HSUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("Changing the username is not supported.");
        }

        public async Task<IdentityResult> UpdateAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var query = Builders<HSUser>.Filter.Eq(u => u.Id, user.Id);
            var replaceResult = await _userCollection.ReplaceOneAsync(query, user, new UpdateOptions { IsUpsert = false }).ConfigureAwait(false);
            return replaceResult.IsModifiedCountAvailable && replaceResult.ModifiedCount == 1
                ? IdentityResult.Success
                : IdentityResult.Failed();
        }

        public void Dispose() { }

        private void EnsureIndicies()
        {

        }

        public Task AddLoginAsync(HSUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            if (user._logins.Any(x => x.Equals(login)))
            {
                throw new InvalidOperationException("Login already exists.");
            }
            user._logins.Add(new HSUserLogin(login));

            return Task.FromResult(0);

        }

        public Task RemoveLoginAsync(HSUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            var login = new UserLoginInfo(loginProvider, providerKey, string.Empty);
            var loginToRemove = user._logins.FirstOrDefault(x => x.Equals(login));

            if (loginToRemove != null)
            {
                user._logins.Remove(loginToRemove);
            }
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var logins = user._logins.Select(login =>
               new UserLoginInfo(login.LoginProvider, login.ProviderKey, null));
            return Task.FromResult<IList<UserLoginInfo>>(logins.ToList());
        }

        public Task<HSUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }


            var loginQuery = Builders<HSUser>.Filter.ElemMatch(u => u.Logins,
                Builders<HSUserLogin>.Filter.And(
                    Builders<HSUserLogin>.Filter.Eq(lg => lg.LoginProvider, loginProvider),
                    Builders<HSUserLogin>.Filter.Eq(lg => lg.ProviderKey, providerKey)
                )
            );

            return _userCollection.Find(loginQuery).FirstOrDefaultAsync();
        }

        public Task SetPasswordHashAsync(HSUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetEmailAsync(HSUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (user.PrimaryEmail == null)
            {
                user.PrimaryEmail = new HSEmailAddress();
            }
            user.PrimaryEmail.Value = email;

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var primaryEmail = user.PrimaryEmail != null ? user.PrimaryEmail.Value : null;
            return Task.FromResult(primaryEmail);
        }

        public Task<bool> GetEmailConfirmedAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var confirmed = user.PrimaryEmail != null ? user.PrimaryEmail.IsConfirmed() : false;
            return Task.FromResult(confirmed);
        }

        public Task SetEmailConfirmedAsync(HSUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if(user.PrimaryEmail != null)
            {
                if (confirmed)
                {
                    user.PrimaryEmail.SetConfirmed();
                }
                else
                {
                    user.PrimaryEmail.SetUnconfirmed();
                }
            }
            return Task.FromResult(0);
        }

        public Task<HSUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (normalizedEmail == null)
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            var query = Builders<HSUser>.Filter.Eq(u => u.PrimaryEmail.ValueNormaized, normalizedEmail);
            return _userCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(HSUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var primaryEmail = user.PrimaryEmail != null ? user.PrimaryEmail.ValueNormaized : null;
            return Task.FromResult(primaryEmail);
        }

        public Task SetNormalizedEmailAsync(HSUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (normalizedEmail != null && user.PrimaryEmail != null)
            {
                user.PrimaryEmail.ValueNormaized = normalizedEmail;
            }
            return Task.FromResult(0);
        }
    }
}
