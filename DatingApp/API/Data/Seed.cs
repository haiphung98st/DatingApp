using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    
    public class Seed
    {
        private readonly DataContext _dataContext;

        public Seed(DataContext context)
        {
            this._dataContext = context;
        }
        public void SeedUser()
        {
            if (_dataContext.Users.Any()) return;
            var userData = File.ReadAllText("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("W@lcome456"));
                _dataContext.Users.Add(user);    
            }
            _dataContext.SaveChanges();
        }
    }
}
