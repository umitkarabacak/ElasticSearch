using Nest;

namespace ElasticSearch.API.Extensions
{
    public static class ElasticSearchDependencyInjectionExtensions
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services)
        {
            var settings = new ConnectionSettings();
                settings.DefaultIndex("products");
                //settings.BasicAuthentication("username","pwd");

            var client = new ElasticClient(settings);

            services.AddSingleton(client);

            return services;
        }
    }
}
