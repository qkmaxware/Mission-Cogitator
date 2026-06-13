using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kt.Data;

public class RuleDatabase
{

    private JsonSerializerOptions json = IO.Pkg.SerializationOptions;

    private HttpClient client;

    private Dictionary<string, Rule> rules;
    private Dictionary<string, Effect> effects;
    private Dictionary<string, Equipment> equipment;

    public IEnumerable<Effect> AllEffects => effects.Values;
    public IEnumerable<Rule> AllRules => rules.Values;
    public IEnumerable<Equipment> AllEquipment => equipment.Values;

    public RuleDatabase(HttpClient client)
    {
        this.client = client;
        this.rules = new();
        this.effects = new();
        this.equipment = new();
    }

    public Rule? GetValue(string id)
    {
        if (rules.TryGetValue(id, out var rule))
            return rule;
        return null;
    }

    public void Alias(string uid, string @as)
    {
        if (!rules.TryGetValue(uid, out Rule? rule))
            return;
        
        if (rules.ContainsKey(@as))
            return; // Don't replace existing stuff for an ALIAS

        rules[@as] = rule;
    }

    public void AddTeam(Team? team)
    {
        if (team is null || team.Rules is null)
            return;

        foreach (var rule in team.Rules)
        {
            AddRule(rule.Key, rule.Value);
        }
    }

    public void AddRule(string id, Rule rule)
    {
        this.rules[id] = rule;
        if (rule is Effect effect)
        {
            this.effects[id] = effect;
        }
        if (rule is Equipment eq)
        {
            this.equipment[id] = eq;
        }
    }
}