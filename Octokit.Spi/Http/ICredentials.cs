using Octokit.Authentication;

namespace Octokit.Http
{
    public interface ICredentials
    {
        AuthenticationType AuthenticationType { get; }

        string Login { get; }

        string Password { get; }
    }
}