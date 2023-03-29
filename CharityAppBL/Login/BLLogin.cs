using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Base;
using Login;
using ActionResult;
using Auth;
using CharityBackendDL;

namespace CharityAppBL.Login
{
    public class BLLogin : BLBase, IBLLogin
    {
        private IDLLogin _dlLogin;

        private readonly TokenValidationParameters _tokenValidationParameters;
        public BLLogin(IDLLogin dlLogin, TokenValidationParameters tokenValidationParameters)
        {
            _dlLogin = dlLogin;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public ReturnResult Authenthicate(UserLogin user)
        {
            var result = new ReturnResult();
            try
            {
                var userLogin = _dlLogin.GetUserByUsrNameOrId(user.UserName);
                if (userLogin == null || !VerifyPasswordHash(user.Password, userLogin.Password, userLogin.SaltPassword))
                {
                    result.BadRequest(new List<string> { "Tên đăng nhập hoặc mật khẩu sai, vui lòng thử lại" });
                    return result;
                }
                string tokenStr = GenerateToken(userLogin);
                result.Ok(tokenStr);
                return result;

            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
                return result;

            }
        }

        private bool VerifyPasswordHash(string password, string hashPassword, string saltPassword)
        {
            var saltPasswordBytes = Encoding.UTF8.GetBytes(saltPassword);
            var hashPasswordBytes = Convert.FromBase64String(hashPassword);
            using (var hmac = new System.Security.Cryptography.HMACSHA512(saltPasswordBytes))
            {
                var computedHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                if (computedHashBytes == null || hashPasswordBytes == null || computedHashBytes.Length != hashPasswordBytes.Length)
                {
                    return false;
                }
                for (int i = 0; i < computedHashBytes.Length; i++)
                {
                    if (computedHashBytes[i] != hashPasswordBytes[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private string GenerateToken(User user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DatabaseContext.ConfigJwt["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(DatabaseContext.ConfigJwt["Issuer"],
              DatabaseContext.ConfigJwt["Audience"],
              claims,
              expires: DateTime.Now.AddDays(1).Date,
              signingCredentials: credentials);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreatedDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddDays(1).Date // 12h
            };

            int _result = _dlLogin.SaveToken(refreshToken);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }

        public ReturnResult Register(UserRegister user)
        {
            var result = new ReturnResult();

            // kiem tra username da ton tai hay chua
            var _user = _dlLogin.GetUserByUsrNameOrId(user.Username);
            if(_user != null)
            {
                result.BadRequest(new List<string> { "Ten dang nhap da ton tai, vui long thu ten khac" });
                return result;
            }

            user.SaltPassword = GenerateSalt();
            user.Password = CreatePasswordHash(user.Password, user.SaltPassword);
            try
            {
                var _result = _dlLogin.CreateUser(user);
                if(_result != 0)
                {
                    
                    result.CreatedSuccess(_result);
                }
                else
                {
                    result.BadRequest(new List<string> { "Looix" });
                }
            }
            catch (Exception e)
            {
                result.BadRequest(new List<string> { e.Message });
            }
            return result;
        }

        public ReturnResult VerifyToken(TokenRequest tokenRequest)
        {
            var validateResult = ValidateToken(tokenRequest);
            return validateResult;
        }

        private ReturnResult ValidateToken(TokenRequest tokenRequest)
        {
            var result = new ReturnResult();
            var jwtTokenHadler = new JwtSecurityTokenHandler();
            try
            {
                // xac minh co dung la jwt token hay ko
                var tokenVerification = jwtTokenHadler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // xac minh co dung giai ma hay ko
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var check = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!check)
                    {
                        throw new Exception("The security algorithm is not correct");
                    }
                }

                //Check xem token con han hay ko
                var utcExpiryDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
            
                if(expiryDate > DateTime.UtcNow)
                {
                    throw new Exception("Token has not yet expired");
                }

                // kiem tra token co trong database ko
                var storedToken = _dlLogin.GetToken(tokenRequest.RefreshToken);
                if(storedToken == null)
                {
                    throw new Exception("Token does not exist");
                }
                var jti = tokenVerification.Claims.FirstOrDefault(x=>x.Type == JwtRegisteredClaimNames.Jti).Value;

                if(storedToken.JwtId != jti)
                {
                    throw new Exception("Token does not match");

                }

                var user = _dlLogin.GetUserByUsrNameOrId(storedToken.UserId);
                if (user != null)
                {
                    result.Ok(user);
                }
                return result;
            }
            catch (SecurityTokenExpiredException expiredException)
            {
                result.Unauthorized(new List<string>() { expiredException.Message });
                return result;
            }
            catch (Exception e)
            {
                result.BadRequest(new List<string>() { e.Message });
                return result;
            }
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal.ToLocalTime();
        }

        private static double TransformDateTime()
        {
            DateTime currentDateVi = DateTime.UtcNow.AddHours(7);
            DateTime nextDateVi = DateTime.UtcNow.AddDays(1).Date;
            return nextDateVi.Subtract(currentDateVi).TotalSeconds;
        }
    }
}
