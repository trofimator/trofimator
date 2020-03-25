using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asmx.Models
{
    public class AuthenticationModel
    {
        /// <summary>
        ///  Gets or sets the Authority to use when making OpenIdConnect calls.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets a string that represents a valid audience that will be used to check against the token's audience.
        /// </summary>
        public string ValidAudience { get; set; }


        /// <summary />
        /// <param name="authority">the Authority to use when making OpenIdConnect calls.</param>
        /// <param name="validAudience">A valid audience that will be used to check against the token's audience.</param>
        public AuthenticationModel(string authority, string validAudience)
        {
            this.Authority = authority;
            this.ValidAudience = validAudience;
        }
    }
}