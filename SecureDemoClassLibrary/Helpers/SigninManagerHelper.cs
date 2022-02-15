using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SecureDemoClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureDemoClassLibrary.Helpers
{
    public class SigninManagerHelper
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly SignInManager<ApplicationUser>? _signinManager;
        private readonly ILogger<SigninManagerHelper>? _logger;
        //private readonly ILogger _logger;

        public SigninManagerHelper(IServiceProvider? serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _serviceProvider = serviceProvider;

            if (_serviceProvider != null)
            {
                _logger = (ILogger<SigninManagerHelper>?)_serviceProvider.GetService(typeof(ILogger<SigninManagerHelper>));

                _signinManager = (SignInManager<ApplicationUser>?)_serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
            }
            if (_signinManager == null)
            {
                throw new ArgumentNullException(nameof(_signinManager));
            }
        }

        //public async Task<IEnumerable<ApplicationUser>?> GetAllAsync()
        //{
        //    if (_signinManager != null)
        //    {
        //        return await _signinManager.UserManager.ToListAsync();
        //    }
        //    return null;
        //}

        public async Task<ApplicationUser?> SignInAsync(string userName, string password)
        {
            ApplicationUser? user = await new UserManagerHelper(_serviceProvider).GetByUsernameAsync(userName);
            if(user == null)
                user = await new UserManagerHelper(_serviceProvider).GetByEmailAsync(userName);

            if (_signinManager != null && user != null)
            {
                if(await _signinManager.CanSignInAsync(user))
                {
                    SignInResult result = await _signinManager.CheckPasswordSignInAsync(user, password, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(DateTime.Now.ToString() + $" {userName}- login successfull");
                        return user;
                    }
                    _logger.LogInformation(DateTime.Now.ToString() + $" {userName}- login failed");
                }
            }
            return null;
        }



    }
}
