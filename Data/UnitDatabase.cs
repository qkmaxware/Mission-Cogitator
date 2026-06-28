namespace Kt.Data;

public class UnitDatabase
{

    private Dictionary<string, Unit> units;

    public IEnumerable<string> AllIds => units.Keys;
    public IEnumerable<Unit> AllUnits => units.Values;

    public UnitDatabase()
    {
        this.units = new();
    }

    public Unit? GetValue(string id)
    {
        if (units.TryGetValue(id, out var team))   
            return team;
        return null;
    }

    public void AddUnit(string id, Unit team)
    {
        this.units[id] = team;
    }

    public void AddTeam(Team team)
    {
        foreach (var unit in (team.Units ?? Enumerable.Empty<Unit>()))
        {
            if (string.IsNullOrEmpty(unit.Id))
                continue;

            AddUnit(unit.Id, unit);
        }
    }
}