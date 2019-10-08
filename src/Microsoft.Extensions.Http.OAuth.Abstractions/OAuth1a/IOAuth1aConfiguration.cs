using Microsoft.Extensions.Http.OAuth.Model;

namespace Microsoft.Extensions.Http.Abstractions.OAuth1a
{
    public interface IOAuth1aConfiguration
    {
        string ConsumerKey { get; }
        string ConsumerSecret { get; }
        string AccessToken { get; }
        string AccessTokenSecret { get; }

        /// <summary>
        /// Currently only HMAC-SHA1 is supported
        /// </summary>
        SignatureMethodType SignatureMethod { get; }
    }
}
