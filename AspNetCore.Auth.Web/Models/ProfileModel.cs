using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Models
{
    public class ProfileModel
    {
        [Required(ErrorMessage ="Have to supply a display name")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "Have to supply a email")]
        [EmailAddress(ErrorMessage ="Invalid email address")]
        public string Email { get; set; }
    }
}
