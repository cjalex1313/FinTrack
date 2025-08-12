using System.Text;
using Hangfire.Dashboard;

namespace FinTrack.Api.Background.Filters;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;
    public HangfireDashboardAuthFilter(string username, string password)
    {
        _password = password;
        _username = username;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (httpContext.Request.Headers.ContainsKey("Authorization"))
        {
            var header = httpContext.Request.Headers["Authorization"].ToString();
            if (header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                var encodedCredentials = header.Substring("Basic ".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials)).Split(':');

                if (credentials.Length == 2 &&
                    credentials[0] == _username &&
                    credentials[1] == _password)
                {
                    return true;
                }
            }
        }

        // Prompt for credentials
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
        return false;
    }
}