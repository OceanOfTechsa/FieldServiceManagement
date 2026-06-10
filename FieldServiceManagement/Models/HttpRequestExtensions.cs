using FieldServiceManagement.Business.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FieldServiceManagement.Models
{
    public static class HttpRequestExtensions
    {
        public static bool IsDebuggingMode(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;

            if (connection.RemoteIpAddress?.ToString() != "::1" && connection.LocalIpAddress?.ToString() != "127.0.0.1")
                return false;

            return AppSettings.EnvironmentName == Enum.Environment.Development.ToString() && req.Host.Host.ToLower().Contains("localhost");
        }
    }
}
