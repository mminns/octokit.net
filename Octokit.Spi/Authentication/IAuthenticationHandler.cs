using Octokit.Http;
using Octokit.Internal;

namespace Octokit.Authentication
{
    public interface IAuthenticationHandler
    {
        void Authenticate(IRequest request, ICredentials credentials);
    }
}