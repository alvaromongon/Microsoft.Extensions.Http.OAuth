using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Http.Abstractions;
using Microsoft.Extensions.Http.Abstractions.OAuth1a;
using Microsoft.Extensions.Http.OAuth.Model;

namespace Microsoft.Extensions.Http.Implementation.Auth
{
    /// <summary>
    /// https://oauth.net/core/1.0a/
    /// https://tools.ietf.org/html/rfc5849
    /// OAuth provides a method for clients to access server resources on behalf of a resource owner
    /// (such as a different client or an end-user).  
    /// It also provides a process for end-users to authorize third-party access to their server resources 
    /// without sharing their credentials(typically, a username and password pair), using user-agent redirections.
    /// </summary>
    public class OAuth1aProtocol : IAuthProvider
    {
        private const string Scheme = "OAuth";
        private const string OauthVersion = "1.0";

        private readonly IOAuth1aConfiguration _configuration;

        public OAuth1aProtocol(IOAuth1aConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<AuthenticationHeaderValue> AuthenticationHeader(HttpRequestMessage request)
        {
            var headerParameters = new SortedDictionary<string, string>
            {
                { "oauth_consumer_key", _configuration.ConsumerKey },
                { "oauth_nonce", BuildNonce() },
                { "oauth_signature_method", _configuration.SignatureMethod.ToOAuthString() },
                { "oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                { "oauth_token", _configuration.AccessToken },
                { "oauth_version", OauthVersion }
            };

            headerParameters.Add("oauth_signature", BuildSignature(request, headerParameters));

            var parametersAsStringListOrderAsc = headerParameters
                .Select(pair => $"{Uri.EscapeDataString(pair.Key)}=\"{Uri.EscapeDataString(pair.Value)}\"");
                //.OrderBy(item => item);

            var parametersSeparatedByComman = string.Join(", ", parametersAsStringListOrderAsc);

            return Task.FromResult(new AuthenticationHeaderValue(Scheme, parametersSeparatedByComman));            
        }

        /// <summary>
        /// Any approach which produces a relatively random alphanumeric string should be OK here
        /// </summary>
        private static string BuildNonce()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        private string BuildSignature(HttpRequestMessage request, IDictionary<string, string> headerParameters)
        {
            var signatureParameters = BuildSignatureParameters(request, headerParameters);

            // Encode signature parameters
            var encodedSignatureParameters = signatureParameters.Select(
                pair => new KeyValuePair<string, string>(Uri.EscapeDataString(pair.Key), Uri.EscapeDataString(pair.Value)));

            // Convert to string signature parameters
            var encodedSignatureParametersString = string.Join("&", encodedSignatureParameters.Select(
                pair => $"{pair.Key}={pair.Value}"));

            var signatureBaseString = BuildSignatureBaseString(request, encodedSignatureParametersString);

            var signingKey = BuildSigningKey();

            // Build signature
            var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(signingKey));
            var signatureBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString));
            return Convert.ToBase64String(signatureBytes);
        }
        
        private SortedDictionary<string, string> BuildSignatureParameters(HttpRequestMessage request, IDictionary<string, string> headerParameters)
        {
            // Header parameters
            var signatureParameters = new SortedDictionary<string, string>(headerParameters);

            // Content
            if (request.Method == HttpMethod.Post)
            {
                foreach (var pair in GetContentParameters(request.Content))
                {
                    signatureParameters.Add(pair.Key, pair.Value);
                }
            }

            // Query  
            var parsedQuery = HttpUtility.ParseQueryString(request.RequestUri.Query);
            foreach (var key in parsedQuery.AllKeys)
            {
                signatureParameters.Add(key, parsedQuery[key]);
            }

            return signatureParameters;
        }

        private IDictionary<string, string> GetContentParameters(HttpContent content)
        {
            // TODO: IMPLEMENT
            return new Dictionary<string, string>();
        }

        private string BuildSignatureBaseString(HttpRequestMessage request, string encodedSignatureParametersString)
        {
            var signatureBaseStringBuilder = new StringBuilder();

            signatureBaseStringBuilder.Append(request.Method.ToString().ToUpper());
            signatureBaseStringBuilder.Append("&");
            signatureBaseStringBuilder.Append(Uri.EscapeDataString(request.RequestUri.GetLeftPart(UriPartial.Path)));
            signatureBaseStringBuilder.Append("&");
            signatureBaseStringBuilder.Append(Uri.EscapeDataString(encodedSignatureParametersString));

            return signatureBaseStringBuilder.ToString();
        }

        private string BuildSigningKey()
        {
            var signingKeyBuilder = new StringBuilder();

            signingKeyBuilder.Append(Uri.EscapeDataString(_configuration.ConsumerSecret));
            signingKeyBuilder.Append("&");
            signingKeyBuilder.Append(Uri.EscapeDataString(_configuration.AccessTokenSecret));

            return signingKeyBuilder.ToString();
        }

    }
}
