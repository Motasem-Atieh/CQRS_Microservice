using Microsoft.AspNetCore.Identity;
using CQRS_Microservice.Models;

namespace CQRS_Microservice.Services
{
    public class PasswordHashingService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordHashingService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyPassword(User user, string password, string hashedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
        }
    }
}
