using Octokit.Http;

namespace Octokit.Internal
{
    interface IAuthenticationHandler
    {
        void Authenticate(IRequest request, ICredentials credentials);
    }
}