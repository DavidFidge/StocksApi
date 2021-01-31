namespace StocksApi.Core
{
    public static class Constants
    {
        public static string StocksApiSensitiveLogging = "STOCKSAPI_SENSITIVELOGGING";
        public static string StocksApiAsxListedCompaniesUrl = "STOCKSAPI_ASX_LISTED_COMPANIES_URL";
        public static string StocksApiStocksDbConnectionString = "STOCKSAPI_STOCKSDBCONNECTIONSTRING";
        public static string StocksApiClientApplicationOrigin = "STOCKSAPI_CLIENTAPPLICATIONORIGIN";
        public static string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        public static string AspNetCoreEnvironmentDevelopment = "Development";

        public static string[] TrueOrOne = { "true", "1" };
    }
}
