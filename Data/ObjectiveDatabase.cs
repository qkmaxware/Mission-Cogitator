using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kt.Data;

public class ObjectiveDatabase
{
    private Dictionary<string, Objective> objectives = new();
    private Dictionary<string, Objective> primary = new();
    private Dictionary<string, Objective> secondary = new();
    

    public IEnumerable<Objective> All => objectives.Values;
    public IEnumerable<Objective> Primary => primary.Values;
    public IEnumerable<Objective> Secondary => secondary.Values;

    public Objective? GetValue(string? id)
    {
        if (id is null)
            return null;
            
        if (objectives.TryGetValue(id, out var rule))
            return rule;
        return null;
    }

    public void AddObjective(string id, Objective rule)
    {
        this.objectives[id] = rule;
        if (rule.Kind == ObjectiveCategory.Primary)
        {
            this.primary[id] = rule;
        }
        if (rule.Kind == ObjectiveCategory.Secondary)
        {
            this.secondary[id] = rule;
        }
    }
}