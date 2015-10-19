﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;
using Octokit.Http;
using Octokit.Internal;

namespace Octokit
{
    /// <summary>
    /// Represents a HTTP 403 - Forbidden response returned from the API.
    /// </summary>
#if !NETFX_CORE
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "These exceptions are specific to the GitHub API and not general purpose exceptions")]
    public class ForbiddenException : ApiException
    {
        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="jsonSerializer">Used to deserialize error response payloads</param>
        public ForbiddenException(IResponse response, IJsonSerializer jsonSerializer) : this(response, null, jsonSerializer)
        {
        }

        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        /// <param name="jsonSerializer">Used to deserialize error response payloads</param>
        public ForbiddenException(IResponse response, Exception innerException, IJsonSerializer jsonSerializer)
            : base(response, innerException, jsonSerializer)
        {
            Debug.Assert(response != null && response.StatusCode == HttpStatusCode.Forbidden,
                "ForbiddenException created with wrong status code");
        }

        public override string Message
        {
            get { return ApiErrorMessageSafe ?? "Request Forbidden"; }
        }
    
#if !NETFX_CORE
        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.
        /// </param>
        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
