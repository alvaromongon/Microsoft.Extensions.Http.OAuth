using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Abstractions;
using Microsoft.Extensions.Http.Abstractions.OAuth1a;
using Microsoft.Extensions.Http.Implementation.Auth;
using Microsoft.Extensions.Http.IntegrationTests.TestTools.Auth;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Extensions.Http.IntegrationTests.Auth
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-2.2#use-ihttpclientfactory-in-a-console-app
    /// </summary>
    public class SingleUserOAuthProviderTests
    {
        private readonly IHost _host;

        public SingleUserOAuthProviderTests()
        {
            var builder = new HostBuilder() 
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IOAuth1aConfiguration, TwitterSingleUserOAuthConfiguration>();
                services.AddSingleton<IAuthProvider, OAuth1aProtocol>();
                services.AddHttpClient();
            }).UseConsoleLifetime();

            _host = builder.Build();
        }

        [Fact]
        public async Task When_ExecuteRequest_WithCorrectCredentials_RequestAccepted()
        {            
            using (var serviceScope = _host.Services.CreateScope())
            {
                // Arrange
                var httpMethod = HttpMethod.Get;

                // TODO: Configurable, use endpoint supported OAuth1a. Do not display twitter here.
                var requestUri = new Uri("https://api.twitter.com/1.1/statuses/home_timeline.json?include_entities=true&page=1");

                var services = serviceScope.ServiceProvider;
                var httpClient = services.GetRequiredService<IHttpClientFactory>().CreateClient();
                var sut = services.GetRequiredService<IAuthProvider>();

                var request = new HttpRequestMessage(httpMethod, requestUri);

                request.Headers.Authorization = await sut.AuthorizationHeader(request);

                // Act
                var result = await httpClient.SendAsync(request);

                // Assert
                Assert.True(result.IsSuccessStatusCode);
            }
        }                
    }    
}
