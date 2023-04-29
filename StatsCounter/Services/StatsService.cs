using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StatsCounter.Models;

namespace StatsCounter.Services
{
    public interface IStatsService
    {
        Task<RepositoryStats> GetRepositoryStatsByOwnerAsync(string owner, CancellationToken cs);
    }
    
    public class StatsService : IStatsService
    {
        private readonly IGitHubService _gitHubService;

        public StatsService(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        public async Task<RepositoryStats> GetRepositoryStatsByOwnerAsync(string owner, CancellationToken cs)
        {
            var repos = (await _gitHubService.GetRepositoryInfosByOwnerAsync(owner, cs)).ToList();
            if (repos.Count < 1)
                return null;
            
            var letterCounts = new Dictionary<char, int>();
            long stargazersCount = 0, watchersCount = 0, forksCount = 0, sizeCount = 0;

            foreach (var repo in repos)
            {
                foreach (var c in repo.Name.ToLower().Where(char.IsLetter))
                {
                    letterCounts.TryAdd(c, 0);
                    letterCounts[c]++;
                }

                stargazersCount += repo.StargazersCount;
                watchersCount += repo.WatchersCount;
                forksCount += repo.ForksCount;
                sizeCount += repo.Size;
            }
            
            var reposCount = repos.Count;

            return new RepositoryStats
            {
                Owner = owner,
                Letters = letterCounts,
                AvgStargazers = (double)stargazersCount / reposCount,
                AvgWatchers = (double)watchersCount / reposCount,
                AvgForks = (double)forksCount / reposCount,
                AvgSize = (double)sizeCount / reposCount
            };
        }
    }
}