using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NotificationService.WebApi.Common.Options;
using NotificationService.WebApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace NotificationService.Tests.Tests
{
    public class AuthenticationServiceTests
    {
        private const string TestEmail = "user@example.com";
        private const string ValidIssuer = "TestIssuer";
        private const string ValidKey = "supersecretkey12345678901234567890";
        private const int LifeTimeMinutes = 30;

        private IOptions<JWTOptions> _jwtOptionsMock;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            var jwtOptionsMock = new Mock<IOptions<JWTOptions>>();
            jwtOptionsMock.Setup(x => x.Value).Returns(new JWTOptions
            {
                Issuer = ValidIssuer,
                Key = ValidKey,
                LifeTimeFromMinutes = LifeTimeMinutes
            });
            _jwtOptionsMock = jwtOptionsMock.Object;

            _authService = new AuthenticationService(_jwtOptionsMock);
        }

        [Test]
        public void GetJWT_ValidEmail_TokenCanBeReadAndValidated()
        {
            var token = _authService.GetJWT(TestEmail);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = ValidIssuer,
                ValidateAudience = true,
                ValidAudience = TestEmail,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            Assert.DoesNotThrow(() =>
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            });
        }
    }
}
