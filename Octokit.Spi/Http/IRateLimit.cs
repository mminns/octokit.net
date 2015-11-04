using System;

namespace Octokit.Http
{
    public interface IRateLimit
    {
        /// <summary>
        /// The maximum number of requests that the consumer is permitted to make per hour.
        /// </summary>
        int Limit { get; }

        /// <summary>
        /// Allows you to clone RateLimit
        /// </summary>
        /// <returns>A clone of <seealso cref="IRateLimit"/></returns>
        IRateLimit Clone();

        /// <summary>
        /// The number of requests remaining in the current rate limit window.
        /// </summary>
        int Remaining { get; }

        /// <summary>
        /// The date and time at which the current rate limit window resets
        /// </summary>
        DateTimeOffset Reset { get; }

        /// <summary>
        /// The date and time at which the current rate limit window resets - in UTC epoch seconds
        /// </summary>
        long ResetAsUtcEpochSeconds { get; }
    }
}