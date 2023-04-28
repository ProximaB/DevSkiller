# StatsCounter

In this task you will finish a simple REST service which allows to return statistics of GitHub user repositories.
The statistics include:

* all kinds of letters used in repository names, case insensitive (for example: ‘a’ was used 108 times, ‘b’ - 30, ‘c’ - 42, etc.),
* average count of stargazers,
* average count of watchers,
* average count of forks,
* average size of the repositories.

The statistics are returned from an endpoint available under `GET /repositories/{owner}` in the following fashion:

```csharp
{
    "owner": "...",
    "letters": {
        "a": 3,
        "b": 4,
        // ...
    },
    "avgStargazers": 0.0,
    "avgWatchers": 0.0,
    "avgForks": 0.0,
    "avgSize": 0.0
}
```

Your task is to provide implementations of 3 methods:

* `StartupExtensions.AddGitHubService` located in `Extensions` directory,
* `GitHubService.GetRepositoryInfosByOwnerAsync` located in `Services` directory,
* `StatsService.GetRepositoryStatsByOwnerAsync` located in `Services` directory.

The methods should contain the following logic:

* `StartupExtensions.AddGitHubService` - DI registration of `GitHubService` as `IGitHubService` and its dependencies using provided parameters,
* `GitHubService.GetRepositoryInfosByOwnerAsync` - retrieving information about repositories for owner directly from GitHub (you don't have to worry about pagination),
* `StatsService.GetRepositoryStatsByOwnerAsync` - processing of data provided by GitHubService in order to count required statistics.

GitHub API reference can be found at: https://developer.github.com/v3/

There are some tests provided for you in the following projects:

* `StatsCounter.Test.Integration`,
* `StatsCounter.Test.Unit`.

They are supposed to make development easier for you, but if you find them obstructing, feel free to modify or remove them (though you shouldn't). Feel free to add more tests, if that makes development easier for you. Modifications in these test projects will not affect your final score.

You may use anything compatible with .NET 6.0.

**Warning** - Please do not modify any existing code in `StatsCounter` project apart from methods specified above. Modifications to class or field/property names may cause verification tests to fail and lower your score.
