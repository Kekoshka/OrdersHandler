using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.WebApi.Interfaces;
using NotificationService.WebApi.Common.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotificationService.WebApi.Services
{
    /// <summary>
    /// Проводит аутентификацию пользователей системы
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Настройки JWT-токенов
        /// </summary>
        JWTOptions _jwtOptions;

        public AuthenticationService(IOptions<JWTOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Аутентифицирует пользователей в системе и выдает JWT-токен
        /// </summary>
        /// <param name="email">Email пользователя, указанный при создании заказа</param>
        /// <returns>JWT-токен</returns>
        public string GetJWT(string email)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: email,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeFromMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
