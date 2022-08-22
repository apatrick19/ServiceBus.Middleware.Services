using Microsoft.Owin.Security.OAuth;
using ServiceBus.Data.ORM.EntityFramework;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ServiceBus.Web
{
    public class CustomAuthProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();


        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            
            using (AiroPayContext obj = new AiroPayContext())
            {
                var userdata = obj.User.FirstOrDefault(x=>x.UserName==context.UserName && x.Password== context.Password);
                if (userdata != null)
                {                   
                    //identity.AddClaim(new Claim(ClaimTypes.Role, userdata.UserRole));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userdata.UserName));
                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "Provided username or  password is incorrect");
                    context.Rejected();
                }
            }
            
        }

    }

}