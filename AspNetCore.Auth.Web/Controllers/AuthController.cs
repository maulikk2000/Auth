using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Auth.Web.Models;
using AspNetCore.Auth.Web.Services;

namespace AspNetCore.Auth.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRegisterUserService _registerUserService;

        public AuthController(IUserService userService, IRegisterUserService registerUserService)
        {
            _userService = userService;
            _registerUserService = registerUserService;
        }
        [Route("signin")]
        public IActionResult SignIn()
        {
            //return View(new SignInModel()); this is for our own login

            //return Challenge(new AuthenticationProperties { RedirectUri = "/" }); //this is when we use just facebook like third party authentication mechanism
            return View();
        }

        [Route("signin/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl = null)
        {
            //return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);

            //return Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, provider);

            var redirectUri = Url.Action("Profile");
            if(returnUrl != null)
            {
                redirectUri += "?returnUrl" + returnUrl;
            }
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, provider);
        }

        [Route("signin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                User user;
                if (await _userService.ValidateCredentials(model.Username, model.Password, out user))
                {
                    await SignInUser(user.Username);
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model); //this model contains the error message as we have created on the view
        }

        [Route("signup")]
        public IActionResult SignUp()
        {
            return View(new SignUpModel());
        }

        [Route("signup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.AddUser(model.Username, model.Password))
                {
                    await SignInUser(model.Username);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Error", "User name cannot be added.. already in use..");
            }
            return View(model); //if some issue with modelstate, pass back to the SignUp page.
        }

        [Route("signout")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,username),
                new Claim("name",username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", null);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }

        [Route("signin/profile")]
        public async Task<IActionResult> Profile(string returnUrl = null)
        {
            var authResult = await HttpContext.AuthenticateAsync("Temporary");

            if (!authResult.Succeeded)
            {
                return RedirectToAction("SignIn");
            }

            //here we use the name identifier of the temporarily signed-in user which resides in principal in auth result
            var user = await _registerUserService.GetUserById(authResult.Principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(user != null)
            {
                return await SignInUser(user, returnUrl);
            }
        }

        private async Task<IActionResult> SignInUser(RegisterUser user, string returnUrl = null)
        {
            //first signout user from temp session
            await HttpContext.SignOutAsync("Temporary");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Redirect(returnUrl == null ? "/" : returnUrl);
        }


    }
}