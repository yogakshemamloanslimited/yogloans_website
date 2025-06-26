using Microsoft.EntityFrameworkCore;
using yogloansdotnet.Models;

namespace yogloansdotnet.Data.Seeders
{
    public class AdminLoginSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminLoginModel>().HasData(
                new AdminLoginModel
                {
                    Id = -1,
                    Username = "admin",
                    Password_hash = "admin123", // Note: In production, this should be hashed
                    CreatedAt = new DateTime(2023, 1, 1) // Use a static date for seeding
                }
            );
        }
    }
}