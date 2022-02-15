using Microsoft.AspNetCore.Identity;
using SecureDemoClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SecureDemoClassLibrary.Helpers
{
    public static class BaseHelper
    {
        private static IServiceProvider? _serviceProvider;
        private static ApplicationUser? _currentUser;
        private static SignInManager<ApplicationUser>? _signInManager;
        private static UserManager<ApplicationUser>? _userManager;
        private static RoleManager<IdentityRole>? _roleManager;

        private static Dictionary<int, int> _userClaims = new Dictionary<int, int>();

        public static IServiceProvider ServiceProvider
        {
            get => _serviceProvider ?? throw new ArgumentNullException(nameof(_serviceProvider) + " has not been initialized");
            set
            {
                _serviceProvider = value;
                Init();
            }
        }

        public static ApplicationUser CurrentUser
        {
            get => _currentUser ?? throw new ArgumentNullException(nameof(_currentUser) + " has not been initialized");
            set
            {
                _currentUser = value;
                LoadClaims().Wait();
            }
        }

        private async static Task LoadClaims()
        {
            IList<string> userRoleNames = await _userManager.GetRolesAsync(CurrentUser);
            foreach (string roleName in userRoleNames)
            {
                IdentityRole role = await _roleManager.FindByNameAsync(roleName);
                IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (Claim claim in roleClaims)
                {
                    if (claim.Value.ToUpper() == "Allow".ToUpper())
                    {
                        int claimKey = (int)Enum.Parse(typeof(UserClaims), claim.Type);

                        if (!_userClaims.ContainsKey(claimKey))
                            _userClaims.Add(claimKey, 0);
                    }
                }
            }

            IList<Claim> userClaims = await _userManager.GetClaimsAsync(CurrentUser);
            foreach (Claim claim in userClaims)
            {
                int claimKey = (int)Enum.Parse(typeof(UserClaims), claim.Type);

                if (claim.Value.ToUpper() == "Deny".ToUpper())
                {
                    if (_userClaims.ContainsKey(claimKey))
                    {
                        _userClaims.Remove(claimKey);
                    }
                }
                else
                {
                    if (_userClaims.ContainsKey(claimKey))
                    {
                        continue;
                    }
                    else
                    {
                        _userClaims.Add(claimKey, 0);
                    }
                }
            }
            
        }

        public static SignInManager<ApplicationUser> SignInManager
        {
            get => _signInManager ?? throw new ArgumentNullException(nameof(_signInManager) + " has not been initialized");
            set => _signInManager = value;
        }

        public static UserManager<ApplicationUser> UserManager
        {
            get => _userManager ?? throw new ArgumentNullException(nameof(_userManager) + " has not been initialized");
            set => _userManager = value;
        }

        public static RoleManager<IdentityRole> RoleManager
        {
            get => _roleManager ?? throw new ArgumentNullException(nameof(_roleManager) + " has not been initialized");
            set => _roleManager = value;
        }


        private static void Init()
        {

            //var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            if (_serviceProvider != null)
            {
                _signInManager = (SignInManager<ApplicationUser>?)_serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
                _userManager = _signInManager?.UserManager;
                _roleManager = (RoleManager<IdentityRole>?)_serviceProvider?.GetService(typeof(RoleManager<IdentityRole>));
            }
        }

        public static async Task<bool> HasPermissionOld(string claimName)
        {
            if (_serviceProvider == null || _currentUser == null)
                throw new Exception("Base Helper class has not been initialized");

            bool result = false;

            if (CurrentUser == null)
            {
                throw new ArgumentNullException(nameof(CurrentUser));
            }
            if (string.IsNullOrWhiteSpace(claimName))
            {
                throw new ArgumentNullException(nameof(claimName));
            }
            if (_userManager != null && _roleManager != null)
            {
                IList<Claim> claims = await _userManager.GetClaimsAsync(CurrentUser);

                var claimList = claims.Where(c => c.Type.ToUpper() == claimName.ToUpper());
                if (claimList != null && claimList.Count() > 0)
                {
                    result = (claimList.First() != null && claimList.First().Value != null && claimList.First().Value.ToUpper() == "Allow".ToUpper());
                    return result;
                }
                else
                {
                    IList<string> memberOfRoles = await _userManager.GetRolesAsync(CurrentUser);

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

        public static bool HasPermission(UserClaims claim)
        {
            return true;
            //return _userClaims.ContainsKey((int)claim);
        }
    }
}
