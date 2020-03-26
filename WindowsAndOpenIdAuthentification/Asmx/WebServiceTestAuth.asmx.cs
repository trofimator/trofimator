using System.Web.Script.Services;
using System.Web.Services;
using Asmx.Models;
using Asmx.Services;

namespace Asmx
{
    /// <summary>
    /// Summary description for WebServiceTestAuth
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceTestAuth : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            string token = this.Context.Request.Headers[Headers.Authorization];

            // Context.User.Identity.IsAuthenticated is true when only Windows authentification is set in the IIS and a user from active directory tries to get access.
            // When anonimus authentification is added in addition to windows authentification
            // Context.User.Identity.IsAuthenticated is always false not depending on the user.
            if (Context.User.Identity.IsAuthenticated || OpenIdJwtExtension.ValidateToken(token, ConfigurationService.ReadConfig()) != null)
            {
                return "Hello World";
            }

            throw new System.UnauthorizedAccessException();
        }

        /*if (context.User.Identity.IsAuthenticated 
                || OpenIdJwtExtension.ValidateTokenAsync(token, config.Authority, config.ValidAudience, config.ClientId) != null)
            {
                await next(context);
                return;
            }

            await context.ChallengeAsync("Windows");
        }*/
    }
}
