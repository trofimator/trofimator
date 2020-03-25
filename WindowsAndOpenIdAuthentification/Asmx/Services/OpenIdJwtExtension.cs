using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Web;
using Asmx.Models;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Asmx.Services
{
    public class OpenIdJwtExtension
    {
        public ClaimsPrincipal ValidateTokenAsync(string jwtToken, Config config)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                return null;
            }

            var splitToken = jwtToken.Split(' ');

            if (splitToken.Length < 2)
            {
                return null;
            }

            var value = new AuthenticationHeaderValue(splitToken[0], splitToken[1]);

            if (value?.Scheme != "Bearer")
            {
                return null;
            }

            var validationParameter = ConfigureAuthentication(new AuthenticationModel(config.Authority, config.ValidAudience));

            ClaimsPrincipal result = null;

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

        private TokenValidationParameters ConfigureAuthentication(AuthenticationModel model)
        {
            var openIdConnectMetadataAddress = $"{model.Authority}/.well-known/openid-configuration";

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
                ValidAudience = model.ValidAudience,
            };

            return tokenValidationParameters;
        }
    }
}