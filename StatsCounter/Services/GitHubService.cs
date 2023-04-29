using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StatsCounter.Models;

namespace StatsCounter.Services
{
    public interface IGitHubService
    {
        Task<IEnumerable<RepositoryInfo>> GetRepositoryInfosByOwnerAsync(string owner, CancellationToken cs);
    }
    
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        
        public GitHubService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GitHubClient");
        }
        public async Task<IEnumerable<RepositoryInfo>> GetRepositoryInfosByOwnerAsync(string owner, CancellationToken cs)
        {
            var response = await _httpClient.GetAsync($"/users/{owner}/repos", cs);
            
            if (response.StatusCode == HttpStatusCode.OK)
                return await response.Content.ReadFromJsonAsync<IEnumerable<RepositoryInfo>>(cancellationToken: cs);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<RepositoryInfo>();
            
            response.EnsureSuccessStatusCode();
            return null;
        }
    }
}
