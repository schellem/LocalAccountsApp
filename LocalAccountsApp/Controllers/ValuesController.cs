using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;

namespace LocalAccountsApp.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public string Get()
        {
            //var userName = this.RequestContext.Principal.Identity.Name;
            //return String.Format("Hello, {0}.", userName);
			 string msg = "";

			 var ip = (ClaimsPrincipal)RequestContext.Principal;
			 
			 /*foreach (Claim c in ip.Claims) {
				msg += c.Type + " : " + c.Value + ",   " ;
			 }*/

			 //var s = ip.Claims.Select(c => c.Type) ;

			 //Return a CSV of of all claim types (names)
			 msg = ip.Claims.Select(c => c.Type).Aggregate((current, next) => current + ", " + next);
			 

			 return msg;

        }
    }
}
