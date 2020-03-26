using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asmx.Models
{
    public class ApiConfiguration
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        // We are considering cloud to on-premises connect scenarios. Due to this fact, ValidAudience does not make sense.
        // The ValidAudience and ClientId in the token will be the same and equal to ClientId.
        // public string ValidAudience { get; set; }
    }
}