using Absence.Domain.Context;
using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace Absence.Domain.Services
{
    public static class DataSeeder
    {
        public static async Task SeedDefaultAdminAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AbsenceContext>();

            await db.Database.MigrateAsync();

            if (await db.Users.AnyAsync()) return;

            var adminUser = new User
            {
                Email = "admin@example.com",
                PasswordHash = Hash("Nwoork.25"),
            };
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();

            var roleAdmin = new Role
            {
                Name = "Admin",
                UserId = adminUser.Id,
            };
            db.Roles.Add(roleAdmin);
            await db.SaveChangesAsync();
        }

        private static string Hash(string pass)
        {
            int SaltSize = 16;
            int HashSize = 32;
            int Iterations = 100000;

            Span<byte> salt = stackalloc byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                pass,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }
    }
}
