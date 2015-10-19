using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Octokit.Http
{
    public interface ICredentialStore
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="Nope")]
        Task<ICredentials> GetCredentials();
    }
}
