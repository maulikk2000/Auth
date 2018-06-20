using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt;

namespace AspNetCore.Auth.Web.Services
{
    public class DummyUserService : IUserService
    {
        private IDictionary<string, (string PasswordHash, User User)> _users =
            new Dictionary<string, (string PasswordHash, User User)>();

        public DummyUserService(IDictionary<string, string> users)
        {
            foreach (var user in users)
            {
                var salt = BCryptHelper.GenerateSalt();
                _users.Add(user.Key.ToLower(), (BCryptHelper.HashPassword(user.Value, salt), new User(user.Key)));
            }
        }

        public Task<bool> AddUser(string username, string password)
        {
            if (_users.ContainsKey(username.ToLower()))
            {
                return Task.FromResult(false);
            }
            string salt = BCryptHelper.GenerateSalt();
            _users.Add(username.ToLower(), (BCryptHelper.HashPassword(password, salt), new User(username)));
            return Task.FromResult(true);
        }

        public Task<bool> ValidateCredentials(string username, string password, out User user)
        {
            user = null;
            var key = username.ToLower();
            if (_users.ContainsKey(key))
            {
                var hash = _users[key].PasswordHash;
                if (BCryptHelper.CheckPassword(password, hash))
                {
                    user = _users[key].User;
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            return Task.FromResult(false);
        }
    }
}
