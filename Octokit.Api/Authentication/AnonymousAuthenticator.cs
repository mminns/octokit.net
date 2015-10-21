using Octokit.Http;

namespace Octokit.Internal
{
    class AnonymousAuthenticator : IAuthenticationHandler
    {
        public void Authenticate(IRequest request, ICredentials credentials)
        {
            // Do nothing. Retain your anonymity.
        }
    }
}
