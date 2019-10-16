
# Introduction 
Microsoft http extension for OAuth.

Currently supporting:
- Application-user authentication: OAuth 1a (access token for user context) 


# Getting Started
Just get it and build it. I was using VS 2019

The idea is to include other types of Oauth authentication making very easy to use the library and avoiding unnecessary code and dependencies.

## Oauth1a
```csharp
var request = new HttpRequestMessage(httpMethod, requestUri);
request.Headers.Authorization = await sut.AuthenticationHeader(request);
var result = await httpClient.SendAsync(request);
```

# Build and Test
Building is easy since there it not special dependecies. 
Just update the settings json file in the integration tests, build and run the tests.
