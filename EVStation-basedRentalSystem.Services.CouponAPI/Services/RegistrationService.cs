using EVStation_basedRentalSystem.Services.AuthAPI.Data;
using EVStation_basedRentalSystem.Services.AuthAPI.Models;
using EVStation_basedRentalSystem.Services.AuthAPI.Models.Dto.Request;
using EVStation_basedRentalSystem.Services.AuthAPI.Models.Dto.Response;
using EVStation_basedRentalSystem.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EVStation_basedRentalSystem.Services.AuthAPI.Service
{
    public class RegistrationService : IRegistrationService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public RegistrationService(AppDbContext db,
                                   UserManager<ApplicationUser> userManager,
                                   EmailService emailService,
                                   IConfiguration config)
        {
            _db = db;
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
        }

        public async Task<RegistrationResponseDto> RegisterUserAsync(RegistrationRequestDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return new RegistrationResponseDto { User = null, Message = "Email already registered." };

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return new RegistrationResponseDto
                {
                    User = null,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Registration failed"
                };

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Renter");

            // Create RenterProfile
            if (await _db.RenterProfiles.FindAsync(user.Id) == null)
            {
                _db.RenterProfiles.Add(new RenterProfile
                {
                    Id = user.Id,
                    UserId = user.Id,
                    FullName = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
            }


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Build email content with token directly
            var subject = "Activate Your Account";
            var body = $@"
Hello {user.Name},<br/><br/>
Here is your activation token: <b>{token}</b><br/>
Please copy this token and paste it into the activation page of our app to activate your account.<br/><br/>
Thank you!
";

            // Send activation email
            await _emailService.SendActivationEmail(user.Email, subject, body);

            // Build User DTO
            var userDto = new Models.Dto.UserDto()
            {
                ID = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Role = "Renter",
                CreatedAt = user.CreatedAt
            };


            return new RegistrationResponseDto
            {
                User = userDto,
                Message = "Registration successful. Please check your email to activate your account."
            };
        }

        public async Task<string> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Invalid user";

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return "Email confirmation failed: " + string.Join(", ", result.Errors.Select(e => e.Description));

            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return "Email confirmed successfully!";
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return "User not found";

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourfrontend.com/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            var body = $"Hello {user.Name},<br/><br/>Reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>";
            await _emailService.SendActivationEmail(user.Email, "Reset Password", body);

            return "Password reset email sent successfully.";
        }

        public async Task<string> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Invalid user";

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return "Password reset failed: " + string.Join(", ", result.Errors.Select(e => e.Description));

            return "Password reset successfully.";
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return "User not found";

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return $"Password change failed: {string.Join(", ", result.Errors.Select(e => e.Description))}";

            return "Password changed successfully";
        }
    }
}
