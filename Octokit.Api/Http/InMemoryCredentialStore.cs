using System.Threading.Tasks;
using Octokit.Http;

namespace Octokit.Internal
{
    public class InMemoryCredentialStore : ICredentialStore
    {
        readonly ICredentials _credentials;

        public InMemoryCredentialStore(ICredentials credentials)
        {
            Ensure.ArgumentNotNull(credentials, "credentials");

            _credentials = credentials;
        }

        public Task<ICredentials> GetCredentials()
        {
            return Task.FromResult(_credentials);
        }
    }
}