using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Services
{
    public class DummyRegisterUserService : IRegisterUserService
    {
        private IDictionary<string, RegisterUser> _users = new Dictionary<string, RegisterUser>();
        public Task<RegisterUser> AddUser(string id, string displayName, string email)
        {
            var user = RegisterUser.Create(id, displayName, email);
            _users.Add(id, user);
            return Task.FromResult(user);

        }

        public Task<RegisterUser> GetUserById(string id)
        {
            if (_users.ContainsKey(id))
            {
                return Task.FromResult(_users[id]);
            }
            return Task.FromResult<RegisterUser>(null);
        }
    }
}
