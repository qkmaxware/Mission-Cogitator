using System.Text.Json.Serialization;

namespace Kt.Data;

/// <summary>
/// A deployment represents the units chosen by the player to be deployed in a specific game
/// </summary>
public class Deployment
{
    /// <summary>
    /// Deployed Equipment
    /// </summary>
    public List<Equipment> Equipment {get; set;} = new ();
    /// <summary>
    /// Deployed units
    /// </summary>
    public List<DeployedUnit> Units {get; set;} = new();
}

public class SerializedDeployment
{
    // Equipment Ids
    public List<string> Equipment {get; set;} = new ();
    // Faction -> List of Units from Faction
    public Dictionary<string, string[]>? Units {get; set;}
}

public enum DeployedUnitOrder
{
    Concealed, Engaged
}

public enum Activation
{
    Inactive, Activated
}

/// <summary>
/// A Unit that has been deployed from a specific KT
/// </summary>
public class DeployedUnit
{

    public DeployedUnit(Team? team, Unit unit)
    {
        this.Team = team;
        this.Unit = unit;

        this.CurrentWounds = 0;
    }

    /// <summary>
    /// KT who owns the unit
    /// </summary>
    public Team? Team {get; set;}

    /// <summary>
    /// The actual unit metadata
    /// </summary>
    public Unit Unit {get; set;}

    private string? _nickname;
    /// <summary>
    /// The unit's nickname or real name if no nickname is set
    /// </summary>
    public string? Nickname
    {
        get => _nickname ?? Unit.Name;
        set => _nickname = string.IsNullOrEmpty(value) ? null : value;
    }


    /// <summary>
    /// The unit's current orders
    /// </summary>
    [JsonIgnore] public DeployedUnitOrder Order {get; set;}

    /// <summary>
    /// The unit's current activation state
    /// </summary>
    [JsonIgnore] public Activation Activation {get; set;}

    /// <summary>
    /// Number of wounds currently hit with
    /// </summary>
    [JsonIgnore]
    public int CurrentWounds
    {
        get => Math.Max(0, _currentWounds);
        set => _currentWounds = Math.Clamp(value, 0, MaxWounds);
    }
    private int _currentWounds;

    [JsonIgnore]
    public int MaxWounds => (Unit?.Attributes?.Wounds ?? 0);

    [JsonIgnore]
    public float HealthPercent => (float)(MaxWounds - CurrentWounds) / (float)MaxWounds;

    [JsonIgnore]
    public bool IsInjured => HealthPercent < 0.5f;

    /// <summary>
    /// Test if the deployed unit is alive or not
    /// </summary>
    [JsonIgnore]
    public bool IsAlive => CurrentWounds < MaxWounds;

    [JsonIgnore]
    public List<Effect>? Effects {get; set;}
}