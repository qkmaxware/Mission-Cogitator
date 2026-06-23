using System.Diagnostics.Metrics;
using Kt.Data.JsInterop;

namespace Kt.Data.IO;

public class PackageManager
{
    private HttpClient client;
    private RuleDatabase ruledb;
    private TeamsDatabase teamdb;
    private JsConsole console;

    private List<Pkg> loaded = new();
    private List<(string, Exception)> failed = new();

    public PackageManager(JsConsole console, HttpClient client, RuleDatabase rules, TeamsDatabase teams)
    {
        this.client = client;
        this.ruledb = rules;
        this.teamdb = teams;
        this.console = console;
    }

    public class PackageLoadException: Exception
    {
        public PackageLoadException(string path, Exception inner): base($"Failed to load package: {path}", inner) {}
    }

    public IEnumerable<Pkg> LoadedPackages => loaded.AsReadOnly();
    public IEnumerable<(string Id, Exception Error)> FailedPackages => failed.AsReadOnly();

    public async Task AddFromUrl(string uri)
    {
        Pkg? pkg;
        try {
            pkg = await Pkg.FromUrl(client, uri);
        } catch (Exception e)
        {
            var outer = new PackageLoadException(uri, e);
            await console.WarnAsync(outer);
            failed.Add((uri, outer));
            return;
        }

        Import(pkg);
    }

    public void Import(Pkg? pkg)
    {
        if (pkg is null)
            return;
        
        loaded.Add(pkg);
        
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

            // Also add a rule for the team
            ruledb.AddRule(team.Id, team.ToTeamDescriptionRule());
        }

        // Add aliasing if it exists
        if (pkg.Aliases is not null)
        {
            foreach (var kv in pkg.Aliases)
            {
                string? uid = kv.Key;
                if (string.IsNullOrEmpty(uid))
                    continue;
                var lst = kv.Value;
                if (lst is null)
                    continue;

                foreach (var alias in lst)
                {
                    if (string.IsNullOrEmpty(alias))
                        continue;

                    ruledb.Alias(uid, @as: alias);
                }
            }
        }
    }
}