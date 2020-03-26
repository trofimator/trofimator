using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using Asmx.Models;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Asmx.Services
{
    public class OpenIdJwtExtension
    {
        public static ClaimsPrincipal ValidateToken(string jwtToken, ApiConfiguration config)
        {
            ClaimsPrincipal result = null;

            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                return result;
            }

            var splitToken = jwtToken.Split(' ');

            if (splitToken.Length < 2)
            {
                return result;
            }

            var value = new AuthenticationHeaderValue(splitToken[0], splitToken[1]);

            if (value?.Scheme != "Bearer")
            {
                return result;
            }

            // We are considering cloud to on-premises connect scenarios. Due to this fact, ValidAudience does not make sense.
            // The ValidAudience and ClientId in the token will be the same and equal to ClientId.
            var validationParameter = ConfigureAuthentication(config.Authority, config.ClientId);

            try
            {
                ClaimsPrincipal claimsPrincipal = null;
                var handler = new JwtSecurityTokenHandler();
                SecurityToken token;

                claimsPrincipal = handler.ValidateToken(value.Parameter, validationParameter, out token);

                if (claimsPrincipal.HasClaim("appid", config.ClientId))
                {
                    result = claimsPrincipal;
                }
            }
            catch (SecurityTokenException)
            {
                result = null;
            }

            return result;
        }

        private static TokenValidationParameters ConfigureAuthentication(string authority, string validAudience)
        {
            var openIdConnectMetadataAddress = $"{authority}/.well-known/openid-configuration";

            var configurationManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    openIdConnectMetadataAddress,
                    new OpenIdConnectConfigurationRetriever());

            var openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None)
                                                   .GetAwaiter()
                                                   .GetResult();


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = openIdConfig.Issuer,
                IssuerSigningKeys = openIdConfig.SigningKeys,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidateAudience = true,
                ValidAudience = validAudience,
            };

            return tokenValidationParameters;
        }
    }
}