using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Login;
using ActionResult;
using Auth;
using CharityBackendDL;
using Newtonsoft.Json;
using System.Net.Http;
using System.Dynamic;
using CharityAppBO.Login;
using Base;
using Microsoft.AspNetCore.Http;

namespace CharityAppBL.Login
{
    public class BLLogin : IBLLogin
    {
        private readonly IDLLogin _dlLogin;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IDLBase dLBase;
        private HttpClient _httpClient = new HttpClient();
        public BLLogin(IDLLogin dlLogin, TokenValidationParameters tokenValidationParameters, IDLBase _dLBase)
        {
            _dlLogin = dlLogin;
            _tokenValidationParameters = tokenValidationParameters;
            dLBase = _dLBase;
        }

        public async Task<ReturnResult> Authenthicate(UserLogin user)
        {
            var result = new ReturnResult();
            try
            {
                var userLogin = _dlLogin.GetUserByUsrNameOrId(user.UserName);
                if (userLogin == null)
                {
                    result.BadRequest(new List<string> { "Tên đăng nhập hoặc mật khẩu sai, vui lòng thử lại" });
                    return result;
                }
                var isLocked = userLogin?.IsLocked;
                if (Convert.ToBoolean(isLocked))
                {
                    result.BadRequest(new List<string> { "Tài khoản này đã bị khoá, vui lòng liên hệ Admin để được giải quyết." });
                    return result;
                }
                var password = userLogin?.Password ?? "";
                var saltPassword = userLogin?.SaltPassword ?? "";

                if (!CharityUtil.VerifyPasswordHash(user.Password, password, saltPassword))
                {
                    result.BadRequest(new List<string> { "Tên đăng nhập hoặc mật khẩu sai, vui lòng thử lại" });
                    return result;
                }
                var _user = CharityUtil.ConvertToType<User>(userLogin);

                string tokenStr = GenerateToken(_user) ?? throw new Exception("Error when create Token");

                var roleId = userLogin?.RoleId;
                if(roleId != null)
                {
                    if (int.Parse(roleId.ToString()) == (int)RoleUser.UserCharity)
                    {
                        userLogin = CharityUtil.ConvertToType<UserCharityReturn>(userLogin);
                    }
                    else if(int.Parse(roleId.ToString()) == (int)RoleUser.UserNormal)
                    {
                        userLogin = CharityUtil.ConvertToType<UserNormalReturn>(userLogin);
                    }
                }
                var objectResult = new
                {
                    Token = tokenStr,
                    User = userLogin
                };
                var objectResult1 = new
                {
                    token = tokenStr,
                    user_id = userLogin.Id
                };
                result.Ok(objectResult);
                var _result = await SendTokenToAPI(objectResult1);
                return result;
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
                return result;

            }
        }

        public async Task<object?> SendTokenToAPI(object requestData)
        {
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("http://host.docker.internal:8089/charity/access/token", content);
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                // Process the response data
                return responseData;
            }
            else
            {
                // Handle the error response
                return null;
            }
        }

        private string GenerateToken(User user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DatabaseContext.ConfigJwt["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("CharityId", user.CharityId.ToString() ?? "")
            };
            var identity = new ClaimsIdentity(claims, "JWT");
            var token = new JwtSecurityToken(DatabaseContext.ConfigJwt["Issuer"],
              DatabaseContext.ConfigJwt["Audience"],
              identity.Claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            var _token = new JwtSecurityTokenHandler().WriteToken(token);
            _dlLogin.SaveToken(user.Id, _token);

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
                result.BadRequest(new List<string> { "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác" });
                return result;
            }
                
            user.SaltPassword = CharityUtil.GenerateSalt();
            user.Password = CharityUtil.CreatePasswordHash(user.Password, user.SaltPassword);
            try
            {
                int _result;
                var excludeColumns = new List<string>() { "InfoCharity", "ConfirmPassword"};
                if (user.RoleId == (int)RoleUser.UserNormal)
                {
                    _result = _dlLogin.CreateUser(user, excludeColumns);
                    if (_result != 0)
                    {
                        result.CreatedSuccess(_result);
                    }
                }
                else if (user.RoleId == (int)RoleUser.UserCharity)
                {
                    if (user.InfoCharity != null)
                    {
                        var idCharity = _dlLogin.CreateCharity(user.InfoCharity);

                        if (idCharity != 0)
                        {
                            user.CharityId = idCharity;
                            _result = _dlLogin.CreateUser(user, excludeColumns);
                            if (_result == 0) {
                                var _rs = _dlLogin.DeleteCharity(idCharity);
                                result.BadRequest(new List<string> { "Dang ki khong thanh cong" });
                                return result;
                            }
                            result.Ok(_result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.BadRequest(new List<string> { e.Message });
            }
            return result;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal.ToLocalTime();
        }

        public ReturnResult Logout(int id)
        {
            var result = new ReturnResult();
            try
            {
                _dlLogin.Logout(id);
                result.Ok(null);
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }

        public async Task<ReturnResult> ForgetPassword(string userName)
        {
            var result = new ReturnResult();
            try
            {
                var _user = _dlLogin.GetUserByUsrNameOrId(userName);
                _user = CharityUtil.ConvertToType<User>(_user);

                if (_user != null && _user?.Email != null && _user?.Id != null)
                {
                    var email = _user?.Email.ToString();
                    var id = _user?.Id.ToString();
                    var resetCode = CharityUtil.GenerateSalt(6);
                    await Task.Run(() => {
                        _dlLogin.SaveResetCode($"{id}_reset", resetCode);
                        CharityUtil.SendMailKit(email, "Mã xác minh nè", resetCode);
                    });
                    var objReturn = new
                    {
                        Email = email,
                        Id = id
                    };
                    var emailHide = HideEmail(email);
                    result.Ok(objReturn);
                    result.Messages = new List<string>() { $"Mã xác nhận đã được gửi tới {emailHide}. Mã này có hiệu lực trong vòng 5 phút."};
                }
                else
                {
                    result.BadRequest(new List<string> { "Tên đăng nhập này không có, vui lòng thử lại." });
                }
            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }

            return result;
        }

        private string HideEmail(string email)
        {
            // Find the position of the "@" symbol
            int atIndex = email.IndexOf("@");

            if (atIndex > 1)
            {
                // Get the first character and the domain part of the email
                string firstCharacter = email.Substring(0, 3);
                string domain = email.Substring(atIndex);

                // Create the hidden portion
                string hiddenPart = new string('*', atIndex - 1);

                // Concatenate the parts together
                return $"{firstCharacter}{hiddenPart}{domain}";
            }

            // Return the original email if it does not meet the requirements for hiding
            return email;
        }


        public ReturnResult ResetPassword(ResetPassword resetPassword)
        {
            var result = new ReturnResult();
            try
            {
                string saltPassword = CharityUtil.GenerateSalt();
                string password = CharityUtil.CreatePasswordHash(resetPassword.Password, saltPassword);

                dynamic resetPasswordObj = new ExpandoObject();
                Dictionary<string, string> updateColumns = new()
                    {

                        {"Password", password },
                        {"SaltPassword", saltPassword }
                    };

                Dictionary<string, OperatorWhere> whereCondition = new()
                    {
                        {"Id", new OperatorWhere(){ Operator= CharityAppBO.Operator.Equal, Value = resetPassword.Id.ToString() } }
                    };

                var _rs = dLBase.Update("user_account", updateColumns, whereCondition);
                if (_rs > 0)
                {
                    result.Ok(_rs);
                }
                else
                {
                    result.BadRequest(new List<string> { "Update không thành công" });
                }

            }
            catch (Exception e)
            {
                result.InternalServer(new List<string> { e.Message });
            }
            return result;
        }

        public ReturnResult ResetCode(ResetCode resetCode)
        {
            var result = new ReturnResult();
            var code = dLBase.GetDataRedis<string>($"{resetCode.Id}_reset");
            if(code != null)
            {
                if(code == resetCode.Code)
                {
                    result.Ok(null);
                }
                else
                {
                    result.BadRequest(new List<string> { "Mã xác nhận không đúng, vui lòng kiểm tra lại" });
                }
            }
            else
            {
                result.BadRequest(new List<string> { "Mã xác nhận không đúng, vui lòng kiểm tra lại" });
            }
            return result;
        }
    }
}
