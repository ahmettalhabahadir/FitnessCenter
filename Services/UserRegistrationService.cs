using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenter.Services
{
    public class UserRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRegistrationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                // Assign Member role by default
                await _userManager.AddToRoleAsync(user, "Member");
            }

            return result;
        }
    }
}

