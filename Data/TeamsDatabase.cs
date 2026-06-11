using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kt.Data;

public class TeamsDatabase
{

    public struct TeamInfo
    {
        public string? Name;
        public string? Group;
        public string? Path;
    }

    private Dictionary<string, Team> teams;

    public IEnumerable<string> AllIds => teams.Keys;
    public IEnumerable<Team> AllTeams => teams.Values;

    public IEnumerable<IGrouping<string?, string>> AllGrouped => teams.Select(t => new KeyValuePair<string, TeamInfo>(t.Key, new TeamInfo { Name = t.Value.Name, Group = t.Value.Faction })).GroupBy(p => p.Value.Group, p => p.Key);

    public IEnumerable<IGrouping<string?, Team>> AllTeamsGrouped => teams.GroupBy(p => p.Value.Faction, p => p.Value);

    public TeamsDatabase()
    {
        this.teams = new();

        // Load all the jsonc files

        // Generic stuffs
        //AddHttp("T'Au Empire", "assets/teams/tau/pathfinders.jsonc");
    }

    public Team? GetValue(string id)
    {
        if (teams.TryGetValue(id, out var team))   
            return team;
        return null;
    }

    public void AddTeam(string id, Team team)
    {
        this.teams[id] = team;
    }
}