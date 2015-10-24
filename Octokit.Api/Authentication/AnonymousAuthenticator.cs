using Octokit.Http;
using Octokit.Internal;

namespace Octokit.Authentication
{
    class AnonymousAuthenticator : IAuthenticationHandler
    {
        public void Authenticate(IRequest request, ICredentials credentials)
        {
            // Do nothing. Retain your anonymity.
        }
    }
}
