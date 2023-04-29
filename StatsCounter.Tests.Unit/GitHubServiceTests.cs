using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using StatsCounter.Models;
using StatsCounter.Services;
using Xunit;

namespace StatsCounter.Tests.Unit
{
    public class GitHubServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly IGitHubService _gitHubService;

        public GitHubServiceTests()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            string clientName = "GitHubClient";
            var httpClient = new HttpClient(_httpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(factory => factory.CreateClient(clientName)).Returns(httpClient);

            _gitHubService = new GitHubService(httpClientFactoryMock.Object);
        }
        
        [Fact]
        public async Task ShouldDeserializeResponse()
        {
            // given
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"id\":1,\"name\":\"name\",\"stargazers_count\":2,\"watchers_count\":3,\"forks_count\":4,\"size\":5}]")
                });
            
            // when
            var result = await _gitHubService.GetRepositoryInfosByOwnerAsync("owner", CancellationToken.None);
            
            // then
            result.Should().BeEquivalentTo(
                new List<RepositoryInfo>
                {
                    new RepositoryInfo
                    {
                        Id = 1,
                        Name = "name",
                        StargazersCount = 2,
                        WatchersCount = 3,
                        ForksCount = 4,
                        Size = 5
                    }
                }.AsEnumerable());
        }
        
        [Fact]
        public async Task ShouldHandleEmptyRepositoriesList()
        {
            // given
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            // when
            var result = await _gitHubService.GetRepositoryInfosByOwnerAsync("owner", CancellationToken.None);

            // then
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task ShouldHandleNon200StatusCode()
        {
            // given
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // when
            Func<Task> action = async () => await _gitHubService.GetRepositoryInfosByOwnerAsync("owner", CancellationToken.None);

            // then
            await action.Should().ThrowAsync<HttpRequestException>();
        }

        [Fact]
        public async Task ShouldHandleInvalidJsonResponse()
        {
            // given
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("invalid-json")
                });

            // when
            Func<Task> action = async () => await _gitHubService.GetRepositoryInfosByOwnerAsync("owner", CancellationToken.None);

            // then
            await action.Should().ThrowAsync<JsonException>();
        }
    }
}