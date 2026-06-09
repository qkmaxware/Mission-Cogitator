using System.Diagnostics.Metrics;

namespace Kt.Data.IO;

public class PackageManager
{
    private HttpClient client;
    private RuleDatabase ruledb;
    private TeamsDatabase teamdb;

    public PackageManager(HttpClient client, RuleDatabase rules, TeamsDatabase teams)
    {
        this.client = client;
        this.ruledb = rules;
        this.teamdb = teams;
    }

    public async Task AddFromUrl(string uri)
    {
        Pkg pkg = await Pkg.FromUrl(client, uri);

        // Add all offered rules       
        foreach (var rule in pkg.AllRules())
        {
            if (string.IsNullOrEmpty(rule.Id))
                continue;

            ruledb.AddRule(rule.Id, rule);
        }

        // Add all offered teams
        foreach (var team in (pkg.Teams?.Values ?? Enumerable.Empty<Team>()))
        {
            if (team.Id is null)
                continue;
                
            teamdb.AddTeam(team.Id, team);
        }
    }
}