using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Topicality.Client.Application.Services;

namespace Topicality.Client.Application;

public static class ServiceExtensions
{
    //TODO pārbaudīt vai tiek izmantots šajā koroservisā
    public static void ConfigureApplication(this IServiceCollection services)
    {
        services.AddHttpClient();
      //  services.AddScoped<ICcn2Service, ICcn2Service>();
    }
}
