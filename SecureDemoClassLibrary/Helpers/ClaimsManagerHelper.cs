using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SecureDemoClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecureDemoClassLibrary.Helpers
{
    public class ClaimsManagerHelper
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly UserManager<ApplicationUser>? _userManager;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly ILogger<UserManagerHelper>? _logger;


        public ClaimsManagerHelper(IServiceProvider? serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _serviceProvider = serviceProvider;

            if (_serviceProvider != null)
            {
                _logger = (ILogger<UserManagerHelper>?)_serviceProvider.GetService(typeof(ILogger<UserManagerHelper>));

                SignInManager<ApplicationUser>? manager = (SignInManager<ApplicationUser>?)_serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));

                _roleManager = (RoleManager<IdentityRole>?)_serviceProvider.GetService(typeof(RoleManager<IdentityRole>));

                if (manager != null)
                {
                    _userManager = manager.UserManager;
                }
            }
            if (_userManager == null)
            {
                throw new ArgumentNullException(nameof(_userManager));
            }
            if (_roleManager == null)
            {
                throw new ArgumentNullException(nameof(_roleManager));
            }
        }

        public async Task<bool> HasPermission(ApplicationUser currentUser, string claimName)
        {
            bool result = false;

            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }
            if (string.IsNullOrWhiteSpace(claimName))
            {
                throw new ArgumentNullException(nameof(claimName));
            }
            if (_userManager != null && _roleManager != null)
            {
                IList<Claim> claims = await _userManager.GetClaimsAsync(currentUser);

                var claimList = claims.Where(c => c.Type.ToUpper() == claimName.ToUpper());
                if (claimList != null && claimList.Count() > 0)
                {
                    result = (claimList.First() != null && claimList.First().Value != null && claimList.First().Value.ToUpper() == "Allow".ToUpper());
                    return result;
                }
                else
                {
                    IList<string> memberOfRoles = await _userManager.GetRolesAsync(currentUser);
                    
                    foreach (var roleName in memberOfRoles)
                    {
                        IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(roleName));
                        var requiredClaims = roleClaims.Where(c => c.Type.ToUpper() == claimName.ToUpper());
                        if (requiredClaims != null && requiredClaims.Count() > 0)
                        {
                            result = (requiredClaims.First() != null && requiredClaims.First().Value != null && requiredClaims.First().Value.ToUpper() == "Allow".ToUpper());
                            if (result)
                                return result;
                        }
                    }
                }
            }
            return result;
        }
    }
}
