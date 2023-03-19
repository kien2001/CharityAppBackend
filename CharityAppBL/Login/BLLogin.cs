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
                if (userLogin == null || !VerifyPasswordHash(user.Password, userLogin.Password))
                {
                    result.BadRequest(new List<string> { "Tên đăng nhập hoặc mật khẩu sai, vui lòng thử lại" });
                }
                AuthResult tokenStr = GenerateToken(userLogin);
                result.Ok(tokenStr);

            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }

        private bool VerifyPasswordHash(string password, string hashPassword)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hashPassword[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private AuthResult GenerateToken(User user)
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
              expires: DateTime.UtcNow.AddSeconds(60),
              signingCredentials: credentials);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };

            int _result = _dlLogin.SaveToken(refreshToken);

            return new AuthResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken.Token
            };


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


            user.Password = CreatePasswordHash(user.Password);
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
                if (storedToken.IsUsed)
                {
                    throw new Exception("Token has been used");

                }

                if(storedToken.IsRevoked)
                {
                    throw new Exception("Token has been revoked");

                }

                var jti = tokenVerification.Claims.FirstOrDefault(x=>x.Type == JwtRegisteredClaimNames.Jti).Value;

                if(storedToken.JwtId != jti)
                {
                    throw new Exception("Token does not match");

                }

                // update current token
                storedToken.IsUsed = true;

                var columnsUpdate = new Dictionary<string, string>()
                {
                    { "IsUsed" , "1" }
                };

                var whereCondition = new Dictionary<string, OperatorWhere>()
                {
                    { "Id" , new OperatorWhere { Operator = 0, Value = storedToken.Id } }
                };

                int _result = _dlLogin.UpdateRefreshToken(columnsUpdate, whereCondition);


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

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal.ToLocalTime();
        }
    }
}
