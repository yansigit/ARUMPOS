using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ArumModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace DotnetAWSBeanstalkBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("")]
        public User GetUser([FromHeader] string token)
        {
            return ArumModels.Models.User.GetUserByToken(token);
        }

        [HttpPost("Login")]
        public Dictionary<string, string> LoginUser([FromBody] UserEmailPasswordInput userEmailPasswordInput)
        {
            var email = userEmailPasswordInput.Email;
            var password = Hash(userEmailPasswordInput.Password);

            var user = ArumModels.Models.User.GetUserByEmailAndPassword(email, password);

            return new Dictionary<string, string>()
            {
                {"success", "true"},
                {"token", user.Token},
                {"isTempPassword", user.IsTempPassword.ToString()},
            };
        }

        [HttpPost("Register")]
        public Dictionary<string, string> RegisterUser(RegisterParam registeringUser)
        {
            StringValues kakaoToken;
            if (Request.Headers.TryGetValue("kakao_token", out kakaoToken))
            {
                var token = kakaoToken.ToString();
                var (result, resultJson) = CheckKakaoTokenValid(token);

                if (!result) return new Dictionary<string, string> { { "error", "카카오 토큰 검증에 실패했습니다" } };

                var kakaoId = int.Parse(resultJson["id"]);
                return ArumModels.Models.User.SaveUser(new User { KakaoId = kakaoId }).Length > 0 ? new Dictionary<string, string> { { "msg", "성공적으로 카카오 가입되었습니다" } } : new Dictionary<string, string> { { "error", "DB 저장 중 에러가 발생했습니다" } };
            }

            if (ArumModels.Models.User.GetEmailCount(registeringUser.Email) > 0 ||
                ArumModels.Models.User.GetPhoneNumberCount(registeringUser.PhoneNumber) > 0)
            {
                return new Dictionary<string, string>()
                {
                    { "error", "이미 이메일이나 휴대폰번호가 존재합니다" },
                    { "status", "false" }
                };
            }
            
            var user = new User
            {
                Email = registeringUser.Email, Password = Hash(registeringUser.Password),
                Name = registeringUser.Name, PhoneNumber = registeringUser.PhoneNumber
            };
            var userToken = ArumModels.Models.User.SaveUser(user);
            return new Dictionary<string, string>
            {
                { "msg", "성공적으로 일반 가입되었습니다" },
                { "token", userToken }
            };
        }

        [HttpDelete("DelAccount/{token}")]
        public Dictionary<string, bool> DeleteAccount(string token)
        {
            ArumModels.Models.User.DeleteAccountWithToken(token);
            return new Dictionary<string, bool>() { { "status", true } };
        }

        [HttpGet("CheckMail/{mail}")]
        public Dictionary<string, bool> CheckMailRedundant(string mail)
        {
            return ArumModels.Models.User.GetEmailCount(mail) > 0 ? new Dictionary<string, bool>() { { "alreadyRegistered", true } } : new Dictionary<string, bool>() { { "alreadyRegistered", false } };
        }

        [HttpGet("CheckPhone/{phone}")]
        public Dictionary<string, bool> CheckPhoneRedundant(string phone)
        {
            return ArumModels.Models.User.GetPhoneNumberCount(phone) > 0 ? new Dictionary<string, bool>() { { "alreadyRegistered", true } } : new Dictionary<string, bool>() { { "alreadyRegistered", false } };
        }

        [HttpGet("Check/Name/{name}/Phone/{phone}")]
        public Dictionary<string, bool> CheckPhoneRedundant(string name, string phone)
        {
            return ArumModels.Models.User.GetPhoneNumberCount(name, phone) > 0 ? new Dictionary<string, bool>() { { "alreadyRegistered", true } } : new Dictionary<string, bool>() { { "alreadyRegistered", false } };
        }

        [HttpGet("StampCheck/{userId:int}/{shopId:int}")]
        public bool GetIsStampValid(int userId, int shopId)
        {
            return ArumModels.Models.User.IsStampsValid(userId, shopId);
        }

        [HttpGet("{userId:int}/MostRedeemedStamp")]
        public Stamp GetMostRedeemedStamp(int userId)
        {
            return ArumModels.Models.User.GetMostRedeemedStamp(userId);
        }

        [HttpGet("{token}")]
        public User GetCurrentUser(string token)
        {
            return ArumModels.Models.User.GetUserByToken(token);
        }

        [HttpGet("FindEmail/{name}/{phone}")]
        public Dictionary<string, string> FindEmail(string name, string phone)
        {
            var email = ArumModels.Models.User.FindEmailAddress(phone, name);
            return new Dictionary<string, string>() { { "email", email } };
        }

        [HttpPost("ChangeEmail")]
        public Dictionary<string, string> ChangeEmail(EmailPhoneParam param, [FromHeader] string token)
        {
            ArumModels.Models.User.ChangeEmailWithToken(token, param.Email);
            return new Dictionary<string, string>() { { "status", "succeed" } };
        }

        [HttpPost("ChangePhoneNumber")]
        public Dictionary<string, string> ChangePhoneNumber(EmailPhoneParam param, [FromHeader] string token)
        {
            ArumModels.Models.User.ChangePhoneNumberWithToken(token, param.Phone);
            return new Dictionary<string, string>() { { "status", "succeed" } };
        }

        [HttpPost("ChangePassword/Token")]
        public Dictionary<string, bool> ChangePassword(TokenPasswordParam param)
        {
            var status = ArumModels.Models.User.ChangePasswordWithToken(param.Token, Hash(param.Password));
            return new Dictionary<string, bool>() { { "status", status } };
        }

        [HttpGet("~/api/CouponCheck/{coupon}")]
        public Dictionary<string, string> CheckCoupon(string coupon, [FromHeader] string token, [FromHeader] string kakao_token)
        {
            if (token is null && kakao_token is null)
            {
                return new Dictionary<string, string>() { { "error", "토큰값이 없습니다" } };
            }

            var c = ArumModels.Models.User.GetCoupon(coupon);
            var couponType = c.Type;

            if (kakao_token is not null)
            {
                return new Dictionary<string, string>()
                    { { "error", "카카오 토큰은 현재 지원하지 않습니다" } };
            }

            var user = ArumModels.Models.User.GetUserByToken(token);
            if (user is null)
            {
                return new Dictionary<string, string>()
                    { { "error", "토큰 정보가 잘못되었습니다" } };
            }

            var canUse = (user.IsAbleToUseCoupon && ArumModels.Models.User.CheckCouponValid(coupon)).ToString();

            return new Dictionary<string, string>()
                { { "couponType", couponType }, { "canUse", canUse.ToLower() } };
        }

        [HttpPost("ChangePassword")]
        public Dictionary<string, bool> ChangePassword(ChangePasswordParam param)
        {
            var (succeed, tempPassword) = ArumModels.Models.User.ChangePasswordTemp(param.Email, param.Name);

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("dalbodre052@gmail.com", "아름드림", System.Text.Encoding.UTF8);
            // 받는이 메일 주소
            mailMessage.To.Add(param.Email);
            // 제목
            mailMessage.Subject = "아름드림 임시 비밀번호 안내";
            // 메일 제목 인코딩 타입(UTF-8) 선택
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            // 본문
            mailMessage.Body = $"임시 비밀번호는 {tempPassword} 입니다.";
            // 본문의 포맷에 따라 선택
            mailMessage.IsBodyHtml = false;
            // 본문 인코딩 타입(UTF-8) 선택
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            // SMTP 서버 주소
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            // SMTP 포트
            SmtpServer.Port = 587;
            // SSL 사용 여부
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            SmtpServer.Credentials = new System.Net.NetworkCredential("dalbodre052", "dkfmaemfla");

            SmtpServer.Send(mailMessage);

            return new Dictionary<string, bool>() { { "status", succeed } };
        }

        [HttpGet("Kakao/Callback")]
        public IDictionary<string, string> KakaoCallBack()
        {
            StringValues code;
            var codeExist = Request.Query.TryGetValue("code", out code);
            if (!codeExist)
            {
                return new Dictionary<string, string> { { "msg", "code 값이 존재하지 않습니다" } };
            }
            
            var httpParams = new Dictionary<string, string>()
            {
                {"grant_type", "authorization_code"},
                {"client_id", "c2e995c6a0b3bd329141b414a9632dce"},
                {"redirect_uri", "https://localhost:44366/api/User/Kakao/Callback"},
                {"code", code.ToString()}
            };

            var response = Program.HttpClient.SendAsync(new HttpRequestMessage
            {
                // Headers = { { "Content-Type", "application/x-www-form-urlencoded" } },
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://kauth.kakao.com/oauth/token"),
                Content = new FormUrlEncodedContent(httpParams)
            }).Result;

            var kakaoTokenResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            if (response.IsSuccessStatusCode)
            {
                var access_token = kakaoTokenResponse.GetValue("access_token").ToString();
            }

            return kakaoTokenResponse.ToObject<Dictionary<string, string>>();
        }

        public static (bool, IDictionary<string, string>) CheckKakaoTokenValid(string token)
        {
            var response = Program.HttpClient.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://kapi.kakao.com/v1/user/access_token_info"),
                Headers =
                {
                    { "Authorization", $"Bearer {token}" }
                }
            }).Result;
            var resJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            return !response.IsSuccessStatusCode ? (false, resJson.ToObject<IDictionary<string, string>>()) : (true, resJson.ToObject<IDictionary<string, string>>());
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

    public class UserEmailPasswordInput
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterParam
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }

    public class ChangePasswordParam
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }

    public class TokenPasswordParam
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }

    public class EmailPhoneParam
    {
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
