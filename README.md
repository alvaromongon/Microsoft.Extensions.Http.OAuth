[![Build Status](https://dev.azure.com/wtwd/Ease%20Maker/_apis/build/status/alvaromongon.Microsoft.Extensions.Http.OAuth?branchName=master)](https://dev.azure.com/wtwd/Ease%20Maker/_build/latest?definitionId=6&branchName=master)

# Introduction 
Microsoft http extension for OAuth.

Currently supporting:
- Application-user authentication: OAuth 1a (access token for user context) 


# Getting Started
Just get it and build it. I was using VS 2019

The idea is to include other types of Oauth authentication making very easy to use the library and avoiding unnecessary code and dependencies.

## Oauth1a
```csharp
IOAuth1aConfiguration configuration = new OAuth1aConfiguration(); // You need to create this implementation
IAuthProvider provider = new OAuth1aProtocol(configuration);

var request = new HttpRequestMessage(httpMethod, requestUri);
request.Headers.Authorization = await provider.AuthenticationHeader(request);
var result = await httpClient.SendAsync(request);
```

# Build and Test
Building is easy since there it not special dependecies. 
Just update the settings json file in the integration tests, build and run the tests.
