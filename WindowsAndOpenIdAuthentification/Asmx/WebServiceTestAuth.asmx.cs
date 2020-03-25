using System.Web.Services;
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

            if (Context.User.Identity.IsAuthenticated || (token != null && new OpenIdJwtExtension().ValidateTokenAsync(token, ConfigurationService.ReadConfig()) != null))
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
