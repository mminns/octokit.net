﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Octokit
{
    public static class StringExtensions
    {
        public static bool IsBlank(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNotBlank(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static Uri FormatUri(this string pattern, params object[] args)
        {
            Ensure.ArgumentNotNullOrEmptyString(pattern, "pattern");

            return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Relative);
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", MessageId = "UriEncode")]
        public static string UriEncode(this string input)
        {
            return WebUtility.UrlEncode(input);
        }

        public static string ToBase64String(this string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string FromBase64String(this string encoded)
        {
            var decodedBytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(decodedBytes, 0, decodedBytes.Length);
        }

        static readonly Regex _optionalQueryStringRegex = new Regex("\\{\\?([^}]+)\\}");

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static Uri ExpandUriTemplate(this string template, object values)
        {
            var optionalQueryStringMatch = _optionalQueryStringRegex.Match(template);
            if (optionalQueryStringMatch.Success)
            {
                var expansion = string.Empty;
                var parameters = optionalQueryStringMatch.Groups[1].Value.Split(new char[] { ',' });

                foreach (var parameter in parameters)
                {
                    var parameterProperty = values.GetType().GetProperty(parameter);
                    if (parameterProperty != null)
                    {
                        expansion += string.IsNullOrWhiteSpace(expansion) ? "?" : "&";
                        expansion += parameter + "=" +
                            Uri.EscapeDataString("" + parameterProperty.GetValue(values, new object[0]));
                    }
                }
                template = _optionalQueryStringRegex.Replace(template, expansion);
            }
            return new Uri(template);
        }

#if NETFX_CORE
        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return type.GetTypeInfo().GetDeclaredProperty(propertyName);
        }
#endif

        // :trollface:
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "Ruby don't care. Ruby don't play that.")]
        public static string ToRubyCase(this string propertyName)
        {
            Ensure.ArgumentNotNullOrEmptyString(propertyName, "s");
            return string.Join("_", propertyName.SplitUpperCase()).ToLowerInvariant();
        }

        static IEnumerable<string> SplitUpperCase(this string source)
        {
            Ensure.ArgumentNotNullOrEmptyString(source, "source");

            int wordStartIndex = 0;
            var letters = source.ToCharArray();
            var previousChar = char.MinValue;

            // Skip the first letter. we don't care what case it is.
            for (int i = 1; i < letters.Length; i++)
            {
                if (char.IsUpper(letters[i]) && !char.IsWhiteSpace(previousChar))
                {
                    //Grab everything before the current character.
                    yield return new String(letters, wordStartIndex, i - wordStartIndex);
                    wordStartIndex = i;
                }
                previousChar = letters[i];
            }

            //We need to have the last word.
            yield return new String(letters, wordStartIndex, letters.Length - wordStartIndex);
        }

        // the rule:
        // Username may only contain alphanumeric characters or single hyphens
        // and cannot begin or end with a hyphen
        static readonly Regex nameWithOwner = new Regex("[a-z0-9.-]{1,}/[a-z0-9.-]{1,}", 
#if (!PORTABLE && !NETFX_CORE)
            RegexOptions.Compiled | 
#endif
            RegexOptions.IgnoreCase);

        public static bool IsNameWithOwnerFormat(this string input)
        {
            return nameWithOwner.IsMatch(input);
        }
    }
}
