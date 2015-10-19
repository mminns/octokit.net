using System;
using System.Globalization;
using Octokit.Http;

namespace Octokit.Internal
{
    class TokenAuthenticator : IAuthenticationHandler
    {
        ///<summary>
        ///Authenticate a request using the OAuth2 Token (sent in a header) authentication scheme
        ///</summary>
        ///<param name="request">The request to authenticate</param>
        ///<param name="credentials">The credentials to attach to the request</param>
        ///<remarks>
        ///See the <a href="http://developer.github.com/v3/#oauth2-token-sent-in-a-header">OAuth2 Token (sent in a header) documentation</a> for more information.
        ///</remarks>
        public void Authenticate(IRequest request, ICredentials credentials)
        {
            Ensure.ArgumentNotNull(request, "request");
            Ensure.ArgumentNotNull(credentials, "credentials");
            Ensure.ArgumentNotNull(credentials.Password, "credentials.Password");

            // TODO the token, is infact stored in the password, its just obscofated
            var token = ((Credentials)credentials).GetToken();
            if (credentials.Login != null)
            {
                throw new InvalidOperationException("The Login is not null for a token authentication request. You " + 
                    "probably did something wrong.");
            }
            if (token != null)
            {
                request.Headers["Authorization"] = string.Format(CultureInfo.InvariantCulture, "Token {0}", token);    
            }
        }
    }
}
