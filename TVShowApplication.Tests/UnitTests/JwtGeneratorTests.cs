using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TVShowApplication.Data.Options;
using TVShowApplication.Services.Authentication;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Tests.UnitTests
{
    internal class JwtGeneratorTests
    {
        private JwtOptions _jwtOptions;
        private IJwtGenerator _jwtGenerator;

        [OneTimeSetUp]
        public void Setup()
        {
            _jwtOptions = new JwtOptions
            {
                Audience = "test",
                Issuer = "test",
                ExpirationSeconds = 60,
                RefreshTokenExpirationDays = 1,
                Secret = "secret",
            };
            _jwtGenerator = new JwtGenerator(Options.Create(_jwtOptions));
        }

        [Test]
        public void GetPrincipal_GivenInvalidJwtToken_ThrowsArgumentException()
        {
            var invalidSignedToken = "this is not a JWT";

            var act = () => _jwtGenerator.GetPrincipalFromExpiredToken(invalidSignedToken);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void GetPrincipal_GivenTokenWithInvalidSigningKey_ThrowsSecurityTokenException()
        {
            var invalidSignedToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ0ZXN0IiwiYXVkIjoidGVzdCJ9.7UPAgfoVuHkcGTaZqlMOgqx_0VG9MMuiiWB7JRSEDMMuqbXS8Dfnm5-n4XhwWWebIkvedcGtRcsB2TvI_aRi9Q";

            var act = () => _jwtGenerator.GetPrincipalFromExpiredToken(invalidSignedToken);

            act.Should().Throw<SecurityTokenException>();
        }
    }
}
