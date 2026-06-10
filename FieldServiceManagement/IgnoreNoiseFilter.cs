using ElmahCore;

public class IgnoreNoiseFilter : IErrorFilter
{
    private readonly IHttpContextAccessor _http;

    public IgnoreNoiseFilter(IHttpContextAccessor http)
    {
        _http = http;
    }

    public void OnErrorModuleFiltering(object sender, ExceptionFilterEventArgs args)
    {
        var ctx = _http.HttpContext;   // ✅ real Microsoft.AspNetCore.Http.HttpContext
        var path = ctx?.Request?.Path.Value ?? "";

        // Ignore some “noise”
        if (path.EndsWith(".map", StringComparison.OrdinalIgnoreCase)) { args.Dismiss(); return; }
        if (path.StartsWith("/.well-known/appspecific", StringComparison.OrdinalIgnoreCase)) { args.Dismiss(); return; }

        // Optional: if you really want to ignore 404s
        if (ctx?.Response?.StatusCode == 404) { args.Dismiss(); return; }
    }
}