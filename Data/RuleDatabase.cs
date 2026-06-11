using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kt.Data;

public class RuleDatabase
{

    private JsonSerializerOptions json = IO.Pkg.SerializationOptions;

    private HttpClient client;

    private struct RuleInfo
    {
        public string? Name;
        public string? Path;
        public Type? Type;
    }

    private Dictionary<string, RuleInfo> paths;
    private Dictionary<string, Rule> rules;

    public IEnumerable<(string Id, string Name)> All => paths.Select(p => (p.Key, p.Value.Name ?? "Unknown Rule")).Concat(rules.Select(p => (p.Key, p.Value.Name ?? "Unknown Rule")));

    public RuleDatabase(HttpClient client)
    {
        this.client = client;
        this.paths = new();
        this.rules = new();

        // Load all the jsonc files

        // Generic stuffs
        AddHttp<Rule>("assets/rules/set-up.jsonc");
        AddHttp<Rule>("assets/rules/phases/strategy-phase.jsonc");
        AddHttp<Rule>("assets/rules/phases/firefight-phase.jsonc");

        // Actions
        AddHttp<Action>("assets/rules/actions/charge.jsonc");
        AddHttp<Action>("assets/rules/actions/dash.jsonc");
        AddHttp<Action>("assets/rules/actions/fall-back.jsonc");
        AddHttp<Action>("assets/rules/actions/fight.jsonc");
        AddHttp<Action>("assets/rules/actions/reposition.jsonc");
        AddHttp<Action>("assets/rules/actions/shoot.jsonc");

        // Definitions
        AddHttp<Rule>("assets/rules/definitions/action.jsonc");
        AddHttp<Rule>("assets/rules/definitions/control-range.jsonc");
        AddHttp<Rule>("assets/rules/definitions/counteract.jsonc");
        AddHttp<Rule>("assets/rules/definitions/cover.jsonc");
        AddHttp<Rule>("assets/rules/definitions/damage.jsonc");
        AddHttp<Rule>("assets/rules/definitions/valid-target.jsonc");
        AddHttp<Rule>("assets/rules/definitions/visible.jsonc");
        AddHttp<Rule>("assets/rules/definitions/gambit.jsonc", "strategic-gambit");

        // Orders
        AddHttp<Rule>("assets/rules/orders/conceal.jsonc");
        AddHttp<Rule>("assets/rules/orders/engage.jsonc");

        // Weapon Rules
        AddHttp<Rule>("assets/rules/weapon-rules/accurate.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/balanced.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/blast.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/brutal.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/ceaseless.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/devastating.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/heavy.jsonc", "heavy-reposition", "heavy-dash", "heavy-charge");
        AddHttp<Rule>("assets/rules/weapon-rules/hot.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/lethal.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/limited.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/piercing.jsonc", "piercing-crits");
        AddHttp<Rule>("assets/rules/weapon-rules/punishing.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/range.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/relentless.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/rending.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/saturate.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/seek.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/severe.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/shock.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/silent.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/stun.jsonc");
        AddHttp<Rule>("assets/rules/weapon-rules/torrent.jsonc");
    }

    public async Task<Rule?> GetValue(string id)
    {
        if (rules.TryGetValue(id, out var rule))   
            return rule;

        if (!paths.TryGetValue(id, out var info))
            return null;
            
        var response = await client.GetAsync(info.Path);
        var stream = response.Content.ReadAsStream();

        return (Rule?)JsonSerializer.Deserialize(stream, info.Type ?? typeof(Rule), json);;
    }

    private void AddHttp<TRule>(string path, params ReadOnlySpan<string> otherAliases)
    where TRule: Rule
    {
        var name = Path.GetFileNameWithoutExtension(path);
        this.paths[name] = new RuleInfo { Name = name, Path = path, Type = typeof(TRule) };
        foreach (var alias in otherAliases)
        {
            this.paths[alias] = new RuleInfo { Name = name, Path = path, Type = typeof(TRule) };
        }
    }

    public void AddTeam(Team? team)
    {
        if (team is null || team.Rules is null)
            return;

        foreach (var rule in team.Rules)
        {
            this.rules[rule.Key] = rule.Value;
        }
    }

    public void AddRule(string id, Rule rule)
    {
        this.rules[id] = rule;
    }
}