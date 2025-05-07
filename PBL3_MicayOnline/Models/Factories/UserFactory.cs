using PBL3_MicayOnline.Models;

namespace PBL3_MicayOnline.Factories
{
    public static class UserFactory
    {
        public static User Create(User baseUser)
        {
            return baseUser.Role switch
            {
                "Admin" => new User_Admin
                {
                    UserId = baseUser.UserId,
                    Username = baseUser.Username,
                    PasswordHash = baseUser.PasswordHash,
                    Email = baseUser.Email,
                    Phone = baseUser.Phone,
                    Role = baseUser.Role,
                    CreatedAt = baseUser.CreatedAt
                },

                "Customer" => new User_Customer
                {
                    UserId = baseUser.UserId,
                    Username = baseUser.Username,
                    PasswordHash = baseUser.PasswordHash,
                    Email = baseUser.Email,
                    Phone = baseUser.Phone,
                    Role = baseUser.Role,
                    CreatedAt = baseUser.CreatedAt
                },

                "Employee" => new User_Employee
                {
                    UserId = baseUser.UserId,
                    Username = baseUser.Username,
                    PasswordHash = baseUser.PasswordHash,
                    Email = baseUser.Email,
                    Phone = baseUser.Phone,
                    Role = baseUser.Role,
                    CreatedAt = baseUser.CreatedAt
                },

                _ => baseUser
            };
        }
    }
}
