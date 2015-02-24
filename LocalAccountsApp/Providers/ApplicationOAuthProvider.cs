using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using LocalAccountsApp.Models;

namespace LocalAccountsApp.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

				//get our users claims to be put in the returned Token
				//The returned Token is accessed on client via data.access_token. 
				//	 Can't read the Tokens contents in client
				//	 To use it pass the raw data back in Authorization header like this 'Bearer ' + data.access_token
				//NOTE: We have to calls to get Claims.....I'm speculating The Cookies on is for Authentication purposes while OAuth is Authorization????


				//I have added a custom claim in the GenerateUserIdentityAsync call (couldn't\didn't figure out how to have the claim come from DB)
				//	 This, and all, claims get returned to client in the Token, then when the Token is passed back from the client the claim is available to back end
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
				

            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

				//These are KVP data that is sent back in the clear and accessed via the returned data 
            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
				//This can be used to gather client credentials from Basic auth
			 //Domick hiut son this briefly in http://www.pluralsight.com/training/player?author=dominick-baier&name=webapi-v2-security-m6a-tokens&mode=live&clip=0&course=webapi-v2-security
			 // in 'Token Based Authentication- Part 1' chapter I think in the sub 'Trusted Applications' or "Demo: Resource Owner Credentail Flow'

            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                //{ "userName", userName }
					 { "userName", userName},
					 {"added", "ByMe"}
            };
            return new AuthenticationProperties(data);
        }
    }
}