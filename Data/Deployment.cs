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
    public class SerializedUnit
    {
        public string? Id {get; set;}
        public string? Nickname {get; set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeployedUnitOrder Orders {get; set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Activation Activation {get; set;}
        public int CurrentWounds {get; set;}
        public List<string?>? Effects {get; set;}
    }

    public List<string?>? Equipment {get; set;} = new ();
    public List<SerializedUnit?>? Units {get; set;} = new ();

    public SerializedDeployment()
    {
        this.Equipment = new List<string?>();
        this.Units = new List<SerializedUnit?>();
    }

    public SerializedDeployment(Deployment? deployment)
    {
        if (deployment == null)
            return;

        Equipment = deployment.Equipment.Select(e => e.Id).ToList();
        Units = deployment.Units.Select(u => new SerializedUnit
        {
            Id = u.Unit.Id,
            Nickname = u.HasNickname ? u.Nickname : null,
            Orders = u.Order,
            Activation = u.Activation,
            CurrentWounds = u.CurrentWounds,
            Effects = u.Effects?.Select(e => e.Id).ToList()
        })
        .Cast<SerializedUnit?>()
        .ToList();
    }

    public Deployment ToDeployment(RuleDatabase ruledb, TeamsDatabase teamdb, UnitDatabase unitdb)
    {
        Deployment deployment = new Deployment();

        deployment.Equipment = Equipment?.Where(eid => !string.IsNullOrEmpty(eid)).Select(eid => ruledb.GetValue(eid!)).Where(e => e != null && e is Equipment).Cast<Equipment>().ToList() ?? new List<Equipment>();
        deployment.Units = Units?.Where(u => u != null && !string.IsNullOrEmpty(u.Id)).Select(u =>
        {
            if (u is null || u.Id is null)
                return null;
                
            var unit = unitdb.GetValue(u.Id);
            if (unit == null)
                return null;

            Team? team = teamdb.AllTeams.FirstOrDefault(t => t.Units?.Contains(unit) ?? t.UnitIds?.Contains(u.Id) ?? false);

            var deployedUnit = new DeployedUnit(team, unit)
            {
                Nickname = u.Nickname,
                Order = u.Orders,
                Activation = u.Activation,
                CurrentWounds = u.CurrentWounds,
                Effects = u.Effects?
                    .Where(eid => !string.IsNullOrEmpty(eid))
                    .Select(eid => ruledb.GetValue(eid!))
                    .Where(e => e != null && e is Effect)
                    .Cast<Effect>()
                    .ToList() ?? new List<Effect>()
            };

            return deployedUnit;
        })
        .Where(u => u != null)
        .Cast<DeployedUnit>()
        .ToList() ?? new List<DeployedUnit>();

        return deployment;
    }

    public void ClearBattleState()
    {
        ClearOrders();
        ClearWounds();
        ClearEffects();
    }

    public void ClearOrders()
    {
        if (Units == null) return;

        foreach (var unit in Units)
        {
            if (unit is null)
                continue;

            unit.Orders = DeployedUnitOrder.Concealed;
            unit.Activation = Activation.Inactive;
        }
    }

    public void ClearWounds()
    {
        if (Units == null) return;

        foreach (var unit in Units)
        {
            if (unit is null)
                continue;

            unit.CurrentWounds = 0;
        }
    }

    public void ClearEffects()
    {
        if (Units == null) return;

        foreach (var unit in Units)
        {
            if (unit is null)
                continue;

            unit.Effects?.Clear();
        }
    }
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
    /// See if the unit has been given a nickname or not
    /// </summary>
    public bool HasNickname => !string.IsNullOrEmpty(_nickname);

    /// <summary>
    /// The unit's current orders
    /// </summary>
    [JsonIgnore] public DeployedUnitOrder Order {get; set;}

    /// <summary>
    /// The unit's current activation state
    /// </summary>
    [JsonIgnore] public Activation Activation {get; set;}

    public bool IsReady => IsAlive && Activation == Activation.Inactive;

    public bool IsActivated => IsAlive && Activation == Activation.Activated;

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