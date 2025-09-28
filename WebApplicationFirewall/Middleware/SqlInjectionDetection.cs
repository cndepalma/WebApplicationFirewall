using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace WebApplicationFirewall.Middleware
{
    public class SqlInjectionDetection
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SqlInjectionDetection> _logger;
        private readonly IFeatureActivatorService _settingsService;

        public SqlInjectionDetection(RequestDelegate next, ILogger<SqlInjectionDetection> logger, IFeatureActivatorService settingsService)
        {
            _next = next;
            _logger = logger;
            _settingsService = settingsService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_settingsService.Settings.SqlInjectionDetectionEnabled)
            {
                var request = context.Request;
                string rawRequestData = ExtractRequestData(request);

                if (IsSqlInjectionAttempt(rawRequestData))


                {
                    _logger.LogWarning($"Detected a SQL injection attempt! Blocked request: {request.Path}");
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden: SQL injection detected.");
                    return;
                }
            }
            await _next(context);
        }

        private string ExtractRequestData(HttpRequest request)
        {
            var requestData = "";

            foreach (var param in request.Query)
            {
                requestData += param.Value + " ";
            }

            foreach (var header in request.Headers)
            {
                requestData += header.Value + " ";
            }

            return requestData.ToLower();
        }

        private bool IsSqlInjectionAttempt(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            var (sqlPatterns, sqlKeywords) = LoadSqlInjectionRules();
            var matchCount = 0;

            foreach (var pattern in sqlPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            foreach (var keyword in sqlKeywords)
            {
                if (input.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }
            return matchCount >= 2;
        }

        private (List<string> Patterns, List<string> Keywords) LoadSqlInjectionRules()
        {
            var path = Path.Combine("Data", "SqlInjectionRules.json");
            var json = File.ReadAllText(path);
            var rules = JObject.Parse(json);
            var patterns = rules["patterns"]?.ToObject<List<string>>() ?? new List<string>();
            var keywords = rules["keywords"]?.ToObject<List<string>>() ?? new List<string>();

            return (patterns, keywords);
        }
    }
}