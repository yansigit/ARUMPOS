using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace ArumModels.Models
{
    public class User : BaseModel
    {
        [Column(TypeName = "varchar(100)")]
        [Required]
        public string Email { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(100)")]
        [JsonIgnore]
        [Required]
        public string Password { get; set; }

        [JsonIgnore]
        [Column(TypeName = "varchar(100)")]
        public string Token { get; set; }

        [JsonIgnore]
        public int KakaoId { get; set; }

        public bool IsAbleToUseCoupon { get; set; } = true;

        public bool IsTempPassword { get; set; }

        public ICollection<Stamp> StampsList { get; set; }

        public bool IsSignedOut { get; set; }

        public static bool DeleteAccountWithToken(string token)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.Token == token);
            user.IsSignedOut = true;
            user.Password = Hash("SIGNED_out_12421521588383");
            user.Token = "$$$_SIGNED_OUT_USER_$$$";
            return db.SaveChanges() > 0;
        }

        public static bool LockUsingCoupon(int userId)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.Id == userId);
            user.IsAbleToUseCoupon = false;
            return db.SaveChanges() > 0;
        }

        public static string FindEmailAddress(string phoneNumber, string name)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.PhoneNumber == phoneNumber && e.Name == name);
            return user.Email;
        }

        public static bool ChangeEmailWithToken(string token, string email)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.Token == token);
            user.Email = email;
            user.IsTempPassword = false;
            return db.SaveChanges() > 0;
        }

        public static bool ChangePhoneNumberWithToken(string token, string phone)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.Token == token);
            user.PhoneNumber = phone;
            user.IsTempPassword = false;
            return db.SaveChanges() > 0;
        }

        public static bool ChangePasswordWithToken(string token, string password)
        {
            using var db = new ArumDbContext();
            var user = db.User.Single(e => e.Token == token);
            user.Password = password;
            user.IsTempPassword = false;
            return db.SaveChanges() > 0;
        }

        public static (bool succeed, string tempPassword) ChangePasswordTemp(string email, string name)
        {
            using var db = new ArumDbContext();
            var random = new Random();
            var user = db.User.Single(e => e.Email == email && e.Name == name);

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var tempPassword = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            user.Password = Hash(tempPassword);

            user.IsTempPassword = true;
            return (db.SaveChanges() > 0, tempPassword);
        }

        public static User GetUser(int userId)
        {
            using var db = new ArumDbContext();
            return db.User.Single(e => e.Id == userId);
        }

        public static int GetPhoneNumberCount(string phone)
        {
            using var db = new ArumDbContext();
            return db.User.Count(e => e.PhoneNumber == phone);
        }

        public static int GetPhoneNumberCount(string name, string phone)
        {
            using var db = new ArumDbContext();
            return db.User.Count(e => e.PhoneNumber == phone && e.Name == name);
        }

        public static int GetEmailCount(string email)
        {
            using var db = new ArumDbContext();
            return db.User.Count(e => e.Email == email);
        }

        public static User GetUserByEmailAndPassword(string email, string pw)
        {
            using var db = new ArumDbContext();
            return db.User.Single(e => e.Email == email && e.Password == pw);
        }

        public static bool IncreaseStamp(int userId, int shopId)
        {
            using var db = new ArumDbContext();
            var stamp = db.User
                .Where(e => e.Id == userId)
                .SelectMany(e => e.StampsList)
                .SingleOrDefault(e => e.Shop.Id == shopId);
            if (stamp == null)
            {
                stamp = new Stamp()
                {
                    Quantity = 0, Shop = db.Shop.Single(e => e.Id == shopId), User = db.User.Single(e => e.Id == userId)
                };
                db.Stamp.Add(stamp);
            }
            stamp.Quantity += 1;
            return db.SaveChanges() > 0;
        }

        public static string GetUserTokenByEmailAndPassword(string email, string password)
        {
            using var db = new ArumDbContext();
            return db.User.Where(e => e.Email == email)
                .Single(e => e.Password == password).Token;
        }

        public static User GetUserByToken(string token)
        {
            using var db = new ArumDbContext();
            return db.User.Include(e => e.StampsList).ThenInclude(e => e.Shop).SingleOrDefault(e => e.Token == token);
        }

        public static User GetUserByKakaoId(int kakaoId)
        {
            using var db = new ArumDbContext();
            return db.User.Single(e => e.KakaoId == kakaoId);
        }

        public static string SaveUser(User entity)
        {
            using var db = new ArumDbContext();
            var rand = new Random();
            if (entity.KakaoId > 0)
            {
                var tmp = new byte[30];
                rand.NextBytes(tmp);
                entity.Password = "*" + Library.Hash.HashSha1(BitConverter.ToString(tmp));
            }
            else
            {
                entity.Token = Library.Hash.HashSha1($"{entity.Email}{entity.Password}{rand.Next()}");
            }

            db.User.Add(entity);
            db.SaveChanges();
            return entity.Token;
        }

        public static bool IsStampsValid(int userId, int shopId)
        {
            using var db = new ArumDbContext();
            var stamp = db.User
                .Where(e => e.Id == userId).SelectMany(e => e.StampsList).Single(e => e.Shop.Id == shopId);
            return stamp.Quantity >= 20;
        }

        public static bool RedeemStamps(int userId, int shopId)
        {
            using var db = new ArumDbContext();
            var stamp = db.User
                .Where(e => e.Id == userId).SelectMany(e => e.StampsList).Single(e => e.Shop.Id == shopId);
            stamp.Quantity -= 20;
            return db.SaveChanges() > 0;
        }

        public static bool CheckCouponValid(string couponCode)
        {
            using var db = new ArumDbContext();
            var coupon = db.Coupon.Single(e => e.CouponCode == couponCode);
            return coupon.Quantity > 0;
        }

        public static Coupon GetCoupon(string couponCode)
        {
            using var db = new ArumDbContext();
            var coupon = db.Coupon.Single(e => e.CouponCode == couponCode);
            return coupon;
        }

        public static bool UseCoupon(string couponCode)
        {
            using var db = new ArumDbContext();
            var coupon = db.Coupon.Single(e => e.CouponCode == couponCode);
            if (coupon.Quantity <= 0) return false;
            coupon.Quantity -= 1;
            return db.SaveChanges() > 0;
        }

        public static Stamp GetMostRedeemedStamp(int userId)
        {
            using var db = new ArumDbContext();
            var stamp = db.User
                .Where(e => e.Id == userId)
                .SelectMany(e => e.StampsList)
                .OrderByDescending(e => e.Redeemed).Single();
            return stamp;
        }

        private static string Hash(string input)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (var b in hash)
            {
                sb.Append(b.ToString());
            }

            return sb.ToString();
        }
    }

    public class Stamp : BaseModel
    {
        public Shop Shop { get; set; }
        public User User { get; set; }
        public int Quantity { get; set; }
        public int Redeemed { get; set; }
    }

    public class Coupon : BaseModel
    {
        public Shop Shop { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string CouponCode { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Type { get; set; }
        public int Quantity { get; set; }
    }
}