using System.Diagnostics.Metrics;

namespace Kt.Data.IO;

public class PackageManager
{
    private HttpClient client;
    private RuleDatabase ruledb;
    private TeamsDatabase teamdb;
    private JsConsole console;

    //private List<string> loadedPkgs = new();

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

    public async Task AddFromUrl(string uri)
    {
        Pkg? pkg;
        try {
            pkg = await Pkg.FromUrl(client, uri);
        } catch (Exception e)
        {
            var outer = new PackageLoadException(uri, e);
            await console.WarnAsync(outer);
            // Don't rethrow this exception since that would break the app, but report it to the log instead.
            return;
        }

        Import(pkg);
    }

    private void Import(Pkg pkg)
    {
        //loadedPkgs.Add(pkg.Name);
        
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