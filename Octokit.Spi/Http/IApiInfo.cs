using System;
using System.Collections.Generic;

namespace Octokit.Http
{
    public interface IApiInfo
    {
        IReadOnlyList<string> OauthScopes { get; }

        /// <summary>
        /// Oauth scopes accepted for this particular call.
        /// </summary>
        IReadOnlyList<string> AcceptedOauthScopes { get; }

        /// <summary>
        /// Etag
        /// </summary>
        string Etag { get; }

        IReadOnlyDictionary<string, Uri> Links { get; }

        IRateLimit RateLimit { get; }

        IApiInfo Clone();
    }
}