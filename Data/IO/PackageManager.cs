using System.Diagnostics.Metrics;
using Kt.Data.JsInterop;

namespace Kt.Data.IO;

public class PackageManager
{
    private HttpClient client;
    private RuleDatabase ruledb;
    private UnitDatabase unitdb;
    private TeamsDatabase teamdb;
    private ObjectiveDatabase objectdb;
    private JsConsole console;

    private List<Pkg> loaded = new();
    private List<(string, Exception)> failed = new();

    public PackageManager(JsConsole console, HttpClient client, RuleDatabase rules, UnitDatabase units, TeamsDatabase teams, ObjectiveDatabase objectives)
    {
        this.client = client;
        this.ruledb = rules;
        this.unitdb = units;
        this.teamdb = teams;
        this.objectdb = objectives;
        this.console = console;
    }

    public class PackageLoadException: Exception
    {
        public PackageLoadException(string path, Exception inner): base($"Failed to load package: {path}", inner) {}
    }

    public IEnumerable<Pkg> LoadedPackages => loaded.AsReadOnly();
    public IEnumerable<IPackagedContent> LoadedContent => loaded.SelectMany(pkg => pkg.Provides());
    public IEnumerable<(string Id, Exception Error)> FailedPackages => failed.AsReadOnly();

    public async Task<Pkg?> AddFromUrl(string uri)
    {
        Pkg? pkg;
        try {
            pkg = await Pkg.FromUrl(client, uri);
        } catch (Exception e)
        {
            var outer = new PackageLoadException(uri, e);
            await console.WarnAsync(outer);
            failed.Add((uri, outer));
            return null;
        }

        Import(pkg);
        return pkg;
    }

    public async Task<Pkg?> AddFromEmbeddedResources(string uri)
    {
        Pkg? pkg;
        try {
            pkg = await Pkg.FromEmbeddedResources(uri);
        } catch (Exception e)
        {
            var outer = new PackageLoadException(uri, e);
            await console.WarnAsync(outer);
            failed.Add((uri, outer));
            return null;
        }

        Import(pkg);
        return pkg;
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

            if (rule is Objective obj && !string.IsNullOrEmpty(obj.Id))
                objectdb.AddObjective(obj.Id, obj);
        }

        // Add all offered teams
        foreach (var unit in (pkg.Units?.Values ?? Enumerable.Empty<Unit>()))
        {
            if (unit.Id is null)
                continue;

            unitdb.AddUnit(unit.Id, unit);
        }
        foreach (var team in (pkg.Teams?.Values ?? Enumerable.Empty<Team>()))
        {
            if (team.Id is null)
                continue;
            
            teamdb.AddTeam(team.Id, team);

            // Resolve all external unit IDs for the team
            team.Units = team.Units ?? new List<Unit>();
            foreach (var id in (team.ExternalUnitIds ?? Enumerable.Empty<string>()))
            {
                var resolved = unitdb.GetValue(id);
                if (resolved is null) {
                    continue;
                }

                team.Units.Add(resolved);
            }
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