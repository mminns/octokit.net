﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Octokit.Http;
using Octokit.Internal;

namespace Octokit
{
    /// <summary>
    /// Represents a "Login Attempts Exceeded" response returned from the API.
    /// </summary>
#if !NETFX_CORE
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "These exceptions are specific to the GitHub API and not general purpose exceptions")]
    public class LoginAttemptsExceededException : ForbiddenException
    {
        /// <summary>
        /// Constructs an instance of LoginAttemptsExceededException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="dataFormatSerializer">Use the deserialize error response payload.</param>
        public LoginAttemptsExceededException(IResponse response, IDataFormatSerializer dataFormatSerializer)
            : base(response, dataFormatSerializer)
        {
        }

        /// <summary>
        /// Constructs an instance of LoginAttemptsExceededException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        /// <param name="dataFormatSerializer">Use the deserialize error response payload.</param>
        public LoginAttemptsExceededException(IResponse response, Exception innerException, IDataFormatSerializer dataFormatSerializer)
            : base(response, innerException, dataFormatSerializer)
        {
        }

        public override string Message
        {
            get { return ApiErrorMessageSafe ?? "Maximum number of login attempts exceeded"; }
        }

#if !NETFX_CORE
        /// <summary>
        /// Constructs an instance of LoginAttemptsExceededException
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.
        /// </param>
        protected LoginAttemptsExceededException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}