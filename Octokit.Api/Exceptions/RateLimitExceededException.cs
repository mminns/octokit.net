﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Octokit.Http;
using Octokit.Internal;

namespace Octokit
{
    /// <summary>
    /// Exception thrown when GitHub API Rate limits are exceeded.
    /// </summary>
    /// <summary>
    /// <para>
    /// For requests using Basic Authentication or OAuth, you can make up to 5,000 requests per hour. For
    /// unauthenticated requests, the rate limit allows you to make up to 60 requests per hour.
    /// </para>
    /// <para>See http://developer.github.com/v3/#rate-limiting for more details.</para>
    /// </summary>
#if !NETFX_CORE
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "These exceptions are specific to the GitHub API and not general purpose exceptions")]
    public class RateLimitExceededException : ForbiddenException
    {
        readonly IRateLimit _rateLimit;

        /// <summary>
        /// Constructs an instance of RateLimitExceededException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="dataFormatSerializer">Use the deserialize error response payload.</param>
        public RateLimitExceededException(IResponse response, IDataFormatSerializer dataFormatSerializer) : this(response, null, dataFormatSerializer)
        {
        }

        /// <summary>
        /// Constructs an instance of RateLimitExceededException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        /// <param name="dataFormatSerializer">Use the deserialize error response payload.</param>
        public RateLimitExceededException(IResponse response, Exception innerException, IDataFormatSerializer dataFormatSerializer) : base(response, innerException, dataFormatSerializer)
        {
            Ensure.ArgumentNotNull(response, "response");

            _rateLimit = response.ApiInfo.RateLimit;
        }

        /// <summary>
        /// The maximum number of requests that the consumer is permitted to make per hour.
        /// </summary>
        public int Limit
        {
            get { return _rateLimit.Limit; }
        }

        /// <summary>
        /// The number of requests remaining in the current rate limit window.
        /// </summary>
        public int Remaining
        {
            get { return _rateLimit.Remaining; }
        }

        /// <summary>
        /// The date and time at which the current rate limit window resets
        /// </summary>
        public DateTimeOffset Reset
        {
            get { return _rateLimit.Reset; }
        }

        // TODO: Might be nice to have this provide a more detailed message such as what the limit is,
        // how many are remaining, and when it will reset. I'm too lazy to do it now.
        public override string Message
        {
            get { return ApiErrorMessageSafe ?? "API Rate Limit exceeded"; }
        }

#if !NETFX_CORE
        /// <summary>
        /// Constructs an instance of RateLimitExceededException
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.
        /// </param>
        protected RateLimitExceededException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _rateLimit = info.GetValue("RateLimit", typeof(RateLimit)) as RateLimit
                         ?? new RateLimit(new Dictionary<string, string>());
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("RateLimit", _rateLimit);
        }
#endif
    }
}