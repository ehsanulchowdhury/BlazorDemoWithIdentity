using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class UserManagerHelper
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly UserManager<ApplicationUser>? _userManager;
        private readonly ILogger<UserManagerHelper>? _logger;

        public UserManagerHelper(IServiceProvider? serviceProvider)
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

                if (manager != null)
                {
                    _userManager = manager.UserManager;
                }

            }
            if (_userManager == null)
            {
                throw new ArgumentNullException(nameof(_userManager));
            }
        }

        public async Task<IEnumerable<ApplicationUser>?> GetAllAsync(ApplicationUser currentUser)
        {
            //if (!BaseHelper.HasPermission(UserClaims.User_View))
            //    throw new Exception("You don't have permission to perform this action");

            if (_userManager != null)
            {
                return await _userManager.Users.ToListAsync();
            }

            return null;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            if (_userManager != null && !string.IsNullOrWhiteSpace(id))
            {
                return await _userManager.FindByIdAsync(id);
            }
            return null;
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            if (_userManager != null && !string.IsNullOrWhiteSpace(username))
            {
                return await _userManager.FindByNameAsync(username);
            }
            return null;
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string username)
        {
            if (_userManager != null && !string.IsNullOrWhiteSpace(username))
            {
                return await _userManager.FindByEmailAsync(username);
            }
            return null;
        }

        public async Task<ApplicationUser> AddUserAsync(ApplicationUser newUser, string passwordToHash)
        {
            DataValidation(newUser);
            if (string.IsNullOrWhiteSpace(passwordToHash))
            {
                throw new ArgumentException("Password is required.");
            }

            if (!BaseHelper.HasPermission(UserClaims.User_Add))
                throw new Exception("You don't have permission to perform this action");

            ApplicationUser user = new ApplicationUser()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                UserEntityType = newUser.UserEntityType,
                UserEntityId = newUser.UserEntityId,
                IsActive = true,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                UserName = newUser.UserName,
            };

            if (string.IsNullOrWhiteSpace(passwordToHash))
                passwordToHash = "Defau1t!";


            if (_userManager != null)
            {
                var result = await _userManager.CreateAsync(user, passwordToHash);
                if (result.Succeeded)
                {
                    return user;
                }
            }

            return new ApplicationUser();

        }

        public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser newUser)
        {
            DataValidation(newUser);

            if (!BaseHelper.HasPermission(UserClaims.User_Edit))
                throw new Exception("You don't have permission to perform this action");

            var user = await GetByIdAsync(newUser.Id);
            if (user == null) throw new ApplicationException("User not found.");

            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            user.UserEntityType = newUser.UserEntityType;
            user.UserEntityId = newUser.UserEntityId;
            user.IsActive = newUser.IsActive;
            user.PhoneNumber = newUser.PhoneNumber;
            user.UserName = newUser.UserName;
            user.Email = newUser.Email;


            if (_userManager != null)
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return user;
                }
            }

            return new ApplicationUser();
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            if (!BaseHelper.HasPermission(UserClaims.User_Delete))
                throw new Exception("You don't have permission to perform this action");

            if (_userManager != null && !string.IsNullOrWhiteSpace(id))
            {
                ApplicationUser? user = await GetByIdAsync(id);

                if (user != null)
                {
                    return await _userManager.DeleteAsync(user);
                }
            }

            return new IdentityResult();
        }

        private void DataValidation(ApplicationUser user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new ArgumentException("First Name is required.");
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new ArgumentException("Last Name is required.");
            }
            if (string.IsNullOrWhiteSpace(user.UserEntityType))
            {
                throw new ArgumentException("User Entity Type is required.");
            }
            if (string.IsNullOrWhiteSpace(user.UserEntityType))
            {
                throw new ArgumentException("User Entity Id is required.");
            }

        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (_userManager != null)
                return await _userManager.GetRolesAsync(user);
            else
                return new List<string>();
        }
        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            if (_userManager == null) throw new ArgumentException(nameof(_userManager));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException(nameof(role));

            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            if (_userManager == null) throw new ArgumentException(nameof(_userManager));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException(nameof(role));

            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddUserClaimsAsync(string userId, string[] userClaimNames)
        {
            if (_userManager == null) throw new ArgumentException(nameof(_userManager));
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException(nameof(userId));

            ApplicationUser? user = await GetByIdAsync(userId);

            if (user != null)
            {
                List<Claim> newClaims = new List<Claim>();
                foreach (var claimName in userClaimNames)
                    newClaims.Add(new Claim(claimName, "Allow"));

                var currentClaims = await _userManager.GetClaimsAsync(user); //Get all current claims
                await _userManager.RemoveClaimsAsync(user, currentClaims); //Remove all current claims
                await _userManager.AddClaimsAsync(user, newClaims); //Add new claims
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> AddUserClaimsAsync(string userId, string[] allowUserClaims, string[] denyUserClaims)
        {
            if (_userManager == null) throw new ArgumentException(nameof(_userManager));
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException(nameof(userId));

            ApplicationUser? user = await GetByIdAsync(userId);

            if (user != null)
            {
                List<Claim> newClaims = new List<Claim>();
                foreach (var claimName in allowUserClaims)
                    newClaims.Add(new Claim(claimName, "Allow"));

                foreach (var claimName in denyUserClaims)
                    newClaims.Add(new Claim(claimName, "Deny"));

                var currentClaims = await _userManager.GetClaimsAsync(user); //Get all current claims
                await _userManager.RemoveClaimsAsync(user, currentClaims); //Remove all current claims
                await _userManager.AddClaimsAsync(user, newClaims); //Add new claims
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            if (_userManager != null)
                return await _userManager.GetClaimsAsync(user);
            else
                return new List<Claim>();

        }

        // Chnage Password
        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            if (!BaseHelper.HasPermission(UserClaims.User_ChangePassword))
                throw new Exception("You don't have permission to perform this action");

            if (_userManager != null && user != null)
            {
                return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            }

            return new IdentityResult();

        }

    }
}
