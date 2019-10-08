namespace Microsoft.Extensions.Http.OAuth.Model
{
    public static class SignatureMethodTypeExtensions
    {
        public static string ToOAuthString(this SignatureMethodType type)
        {
            return type.ToString().Replace("_", "-");
        }
    }
}
