using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http.Abstractions.OAuth1a;
using Microsoft.Extensions.Http.OAuth.Model;

namespace Microsoft.Extensions.Http.OAuth.IntegrationTests.Configuration
{
    public class OAuth1aConfiguration : IOAuth1aConfiguration
    {
        private readonly IConfigurationRoot _configurationRoot;

        public OAuth1aConfiguration(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public string ConsumerKey => _configurationRoot.GetValue<string>(nameof(ConsumerKey));

        public string ConsumerSecret => _configurationRoot.GetValue<string>(nameof(ConsumerSecret));

        public string AccessToken => _configurationRoot.GetValue<string>(nameof(AccessToken));

        public string AccessTokenSecret => _configurationRoot.GetValue<string>(nameof(AccessTokenSecret));

        public SignatureMethodType SignatureMethod => ((SignatureMethodType)_configurationRoot.GetValue<int>(nameof(SignatureMethod)));
    }
}
