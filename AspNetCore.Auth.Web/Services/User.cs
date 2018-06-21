using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Services
{
    public class RegisterUser
    {
        private RegisterUser()
        {

        }

        public static RegisterUser Create(string id, string displayName, string email)
        {
            return new RegisterUser
            {
                Id = id,
                DisplayName = displayName,
                Email = email
            };
        }
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Email { get; private set; }
    }
}
