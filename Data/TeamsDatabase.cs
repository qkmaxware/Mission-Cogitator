using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kt.Data;

public class TeamsDatabase
{

    private JsonSerializerOptions json = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private HttpClient client;

    public struct TeamInfo
    {
        public string? Name;
        public string? Group;
        public string? Path;
    }

    private Dictionary<string, TeamInfo> paths;
    private Dictionary<string, Team> teams;

    public IEnumerable<string> All => paths.Keys.Concat(teams.Keys);

    public IEnumerable<IGrouping<string?, string>> AllGrouped => paths.Concat(teams.Select(t => new KeyValuePair<string, TeamInfo>(t.Key, new TeamInfo { Name = t.Value.Name, Group = t.Value.Faction }))).GroupBy(p => p.Value.Group, p => p.Key);

    public TeamsDatabase(HttpClient client)
    {
        this.client = client;
        this.paths = new();
        this.teams = new();

        // Load all the jsonc files

        // Generic stuffs
        //AddHttp("T'Au Empire", "assets/teams/tau/pathfinders.jsonc");
    }

    public async Task<Team?> GetValue(string id)
    {
        if (teams.TryGetValue(id, out var team))   
            return team;

        if (!paths.TryGetValue(id, out var info))
            return null;
            
        var response = await client.GetAsync(info.Path);
        var stream = response.Content.ReadAsStream();

        return JsonSerializer.Deserialize<Team>(stream, json);
    }

    private void AddHttp(string group, string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        this.paths[name] = new TeamInfo { Name = name, Path = path, Group = group };
    }

    public void AddTeam(string id, Team team)
    {
        this.teams[id] = team;
    }
}