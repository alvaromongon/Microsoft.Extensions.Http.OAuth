using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Http.Abstractions
{
    public interface IAuthProvider
    {
        Task<AuthenticationHeaderValue> AuthorizationHeader(HttpRequestMessage httpRequestMessage);
    }
}
