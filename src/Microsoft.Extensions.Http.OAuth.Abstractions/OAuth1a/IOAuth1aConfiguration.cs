namespace Microsoft.Extensions.Http.Abstractions.OAuth1a
{
    public interface IOAuth1aConfiguration
    {
        string ConsumerKey { get; }
        string ConsumerSecret { get; }
        string AccessToken { get; }
        string AccessTokenSecret { get; }

        string SignatureMethod { get; }
    }
}
