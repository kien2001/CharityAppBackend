using Auth;
using Login;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Validation
{
    public class JwtTokenValidator : ISecurityTokenValidator
    {
        private readonly IDistributedCache _redisCache;

        private readonly IDLLogin _dlLogin;

        public JwtTokenValidator(IDistributedCache distributedCache, IDLLogin dLLogin)
        {
            _redisCache = distributedCache;
            _dlLogin = dLLogin;
        }

        // Implement the CanValidateToken method of ISecurityTokenValidator
        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // Implement the CanReadToken method of ISecurityTokenValidator
        public bool CanReadToken(string token) => true;

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // Implement your token validation logic here
            // For example, you can use the JwtSecurityTokenHandler to validate the token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Validate the token and return the validated token
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // xac minh co dung giai ma hay ko
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var check = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!check)
                    {
                        throw new SecurityTokenException("The security algorithm is not correct");
                    }
                }

                //Check xem token con han hay ko
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.Now)
                {
                    throw new SecurityTokenException("Token has not yet expired");
                }

                Claim userIdClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("UserId claim not found in token");
                int userId = int.Parse(userIdClaim.Value);
                 var storedToken = _dlLogin.GetToken(userId);
                if (storedToken == null)
                {
                    throw new SecurityTokenException("Token does not exist");
                }
                var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (storedToken.JwtId != jti)
                {
                    throw new SecurityTokenException("Token does not match");
                }

                return principal;
            }
            catch (Exception ex)
            {
                // Handle any validation errors
                throw new SecurityTokenException("Failed to validate token", ex);
            }
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal.ToLocalTime();
        }
    }
}
