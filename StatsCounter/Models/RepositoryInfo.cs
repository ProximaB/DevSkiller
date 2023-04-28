using System.Text.Json.Serialization;

namespace StatsCounter.Models
{
    public class RepositoryInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("stargazers_count")]
        public long StargazersCount { get; set; }
        
        [JsonPropertyName("watchers_count")]
        public long WatchersCount { get; set; }
        
        [JsonPropertyName("forks_count")]
        public long ForksCount { get; set; }
        
        [JsonPropertyName("size")]
        public long Size { get; set; }
    }
}