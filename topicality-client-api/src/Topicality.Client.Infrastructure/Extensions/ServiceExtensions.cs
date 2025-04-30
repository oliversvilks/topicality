using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestEase;
using Sentry;
using Topicality.Client.Application.Services;
using Topicality.Client.Infrastructure.Services;
using Topicality.Client.Infrastructure.Configuration;
using Topicality.Domain.Interfaces;

namespace Topicality.Client.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<CCN2ServiceOptions>(configuration.GetSection(CCN2ServiceOptions.CCN2Service));

        // Services
        services.AddTransient<IWeaviateApiService, IWeaviateApiService>();
        services.AddTransient<ICategoryDocumentService, CategoryDocumentService>();
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IAzureBlobService, AzureBlobService>();
    }
}
