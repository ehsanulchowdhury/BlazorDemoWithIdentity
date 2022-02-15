using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecureDemoClassLibrary.Helpers
{
    public class RoleManagerHelper
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly ILogger<RoleManagerHelper>? _logger;

        public RoleManagerHelper(IServiceProvider? serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _serviceProvider = serviceProvider;

            if (_serviceProvider != null)
            {
                _logger = (ILogger<RoleManagerHelper>?)_serviceProvider.GetService(typeof(ILogger<RoleManagerHelper>));

                _roleManager = (RoleManager<IdentityRole>?)_serviceProvider.GetService(typeof(RoleManager<IdentityRole>));
            }
            if (_roleManager == null)
            {
                throw new ArgumentNullException(nameof(_roleManager));
            }
        }

        public async Task<IEnumerable<IdentityRole>?> GetAllAsync()
        {
            if (_roleManager != null)
            {
                return await _roleManager.Roles.OrderBy(r=>r.Name).ToListAsync();
            }
            return null;
        }

        public async Task<IdentityRole?> GetByIdAsync(string id)
        {
            if (_roleManager != null && !string.IsNullOrWhiteSpace(id))
            {
                return await _roleManager.FindByIdAsync(id);
            }
            return null;
        }

        public async Task<IdentityResult> DeleteRoleAsync(string id)
        {
            if (_roleManager != null && !string.IsNullOrWhiteSpace(id))
            {
                IdentityRole? role = await GetByIdAsync(id);

                if (role != null)
                {
                    return await _roleManager.DeleteAsync(role);
                }
            }
            return new IdentityResult();
        }

        public async Task<IdentityRole> AddRoleAsync(IdentityRole identityRole)
        {
            DataValidation(identityRole);

            IdentityRole role = new IdentityRole()
            {
                Name = identityRole.Name,
            };

            if (_roleManager != null)
            {
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return role;
                }
            }
            return new IdentityRole();
        }

        public async Task<IdentityRole> UpdateRoleAsync(IdentityRole identityRole)
        {
            DataValidation(identityRole);

            IdentityRole? role = await GetByIdAsync(identityRole.Id);
            if (role == null) throw new ApplicationException("Role not found.");

            role.Name = identityRole.Name;

            if (_roleManager != null)
            {
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return role;
                }
            }
            return new IdentityRole();
        }

        private void DataValidation(IdentityRole role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                throw new ArgumentException("Role Name is required.");
            }
        }

        public async Task<IList<Claim>> GetClaimsAsync(IdentityRole role)
        {
            if (_roleManager != null)
                return await _roleManager.GetClaimsAsync(role);
            else
                return new List<Claim>();
        }

        public async Task<IdentityResult> AddRoleClaimsAsync(string roleName, string[] allowUserClaims, string[] denyUserClaims)
        {
            if (_roleManager == null) throw new ArgumentException(nameof(_roleManager));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException(nameof(roleName));

            IdentityRole? role = await GetByIdAsync(roleName);

            if (role != null)
            {
                List<Claim> newClaims = new List<Claim>();
                foreach (var claimName in allowUserClaims)
                    newClaims.Add(new Claim(claimName, "Allow"));

                foreach (var claimName in denyUserClaims)
                    newClaims.Add(new Claim(claimName, "Deny"));

                var currentClaims = await _roleManager.GetClaimsAsync(role); //Get all current claims

                foreach (var claim in currentClaims)
                    await _roleManager.RemoveClaimAsync(role, claim); //Remove all current claims

                foreach(var claim in newClaims)
                    await _roleManager.AddClaimAsync(role, claim); //Add new claims

                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

    }
}
