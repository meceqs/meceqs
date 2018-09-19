using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public static class AspNetCoreReceiverUtils
    {
        public static string CombineRoutePrefixAndMessagePath(PathString routePrefix, string path)
        {
            path = "/" + path.TrimStart('/');

            if (routePrefix.HasValue)
            {
                path = "/" + routePrefix.ToString().Trim('/') + path;
            }

            return path;
        }
    }
}
