using CodinovaAssignment.Model;
using CodinovaAssignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodinovaAssignment.Sevices
{
    public interface IUserService
    {
        UserDetails Authenticate(string username, string password);
        IEnumerable<UserDetails> GetAll();
        UserDetails GetById(int id);
        UserDetails Create(UserDetails user, string password);
        void Update(UserDetails user, string password = null);
        void Delete(int id);

    }
    public class UserService : IUserService
    {
        private codinovaTestContext _dbContext;

        public UserService(codinovaTestContext context)
        {
            _dbContext = context;
        }
        public UserDetails Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _dbContext.Users.SingleOrDefault(x => x.UserName == username);
            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            return user;
        }
        public IEnumerable<UserDetails> GetAll()
        {
            return _dbContext.Users;
        }

        public UserDetails GetById(int id)
        {
            return _dbContext.Users.Find(id);
        }

        public UserDetails Create(UserDetails user, string password)
        {

            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_dbContext.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return user;
        }

        public void Update(UserDetails userParam, string password = null)
        {
            var user = _dbContext.Users.Find(userParam.UserId);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {

                if (_dbContext.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var userInfo = _dbContext.Users.Find(id);
            if (userInfo != null)
            {
                _dbContext.Users.Remove(userInfo);
                _dbContext.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }
    }
}



