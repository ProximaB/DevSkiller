using System;
using Microsoft.Extensions.DependencyInjection;
using StatsCounter.Services;

namespace StatsCounter.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddGitHubService(
            this IServiceCollection services,
            Uri baseApiUrl)
        {
            services.AddHttpClient("GitHubClient", client =>
            {
                client.BaseAddress = baseApiUrl;
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                client.DefaultRequestHeaders.Add("User-Agent", "StatsCounter");
            });
            
            services.AddSingleton<IGitHubService, GitHubService>();
            

            return services;
        }
    }
}