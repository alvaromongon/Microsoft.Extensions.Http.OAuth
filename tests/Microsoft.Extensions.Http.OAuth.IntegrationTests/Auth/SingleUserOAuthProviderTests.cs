using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Abstractions;
using Microsoft.Extensions.Http.Abstractions.OAuth1a;
using Microsoft.Extensions.Http.Implementation.Auth;
using Microsoft.Extensions.Http.OAuth.IntegrationTests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Extensions.Http.IntegrationTests.Auth
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-2.2#use-ihttpclientfactory-in-a-console-app
    /// </summary>
    [TestClass]
    public class SingleUserOAuthProviderTests
    {
        private const string _settingsFile = "local.settings.json";

        private string _requestUrl;
        private IHost _host;

        [TestInitialize]
        public void TestInitialize()
        {
            var configurationRoot = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile(_settingsFile, optional: false, reloadOnChange: false)
                    .Build();

            _requestUrl = configurationRoot.GetValue<string>("RequestUrl");

            var builder = new HostBuilder() 
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IConfigurationRoot>(configurationRoot);
                services.AddSingleton<IOAuth1aConfiguration, OAuth1aConfiguration>();
                services.AddSingleton<IAuthProvider, OAuth1aProtocol>();
                services.AddHttpClient();
            }).UseConsoleLifetime();

            _host = builder.Build();
        }

        [TestMethod]
        public async Task When_ExecuteRequest_WithCorrectCredentials_RequestAccepted()
        {            
            using (var serviceScope = _host.Services.CreateScope())
            {
                // Arrange
                var httpMethod = HttpMethod.Get;

                var requestUri = new Uri(_requestUrl);

                var services = serviceScope.ServiceProvider;
                var httpClient = services.GetRequiredService<IHttpClientFactory>().CreateClient();
                var sut = services.GetRequiredService<IAuthProvider>();

                var request = new HttpRequestMessage(httpMethod, requestUri);

                request.Headers.Authorization = await sut.AuthenticationHeader(request);

                // Act
                var result = await httpClient.SendAsync(request);

                // Assert
                Assert.IsTrue(result.IsSuccessStatusCode);
            }
        }                
    }    
}
