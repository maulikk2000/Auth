using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Services
{
    public interface IRegisterUserService
    {
        Task<RegisterUser> GetUserById(string id);
        Task<RegisterUser> AddUser(string id, string displayName, string email);
    }
}
