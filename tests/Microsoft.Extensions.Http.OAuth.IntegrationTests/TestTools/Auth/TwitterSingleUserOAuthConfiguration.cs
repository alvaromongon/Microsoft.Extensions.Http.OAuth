using Microsoft.Extensions.Http.Abstractions.OAuth1a;

namespace Microsoft.Extensions.Http.IntegrationTests.TestTools.Auth
{
    class TwitterSingleUserOAuthConfiguration : IOAuth1aConfiguration
    {
        public string ConsumerKey { get; set; } = "***";
        public string ConsumerSecret { get; set; } = "***";
        public string AccessToken { get; set; } = "***";
        public string AccessTokenSecret { get; set; } = "***";

        public string SignatureMethod { get; set; }
    }
}
