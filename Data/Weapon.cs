using System.Text.Json.Serialization;

namespace Kt.Data;

public class Damage
{
    public int Normal {get; set;}
    public int Critical {get; set;}
}

public enum WeaponKind
{
    ranged, melee
}

public struct WeaponRule
{
    public string? Id {get; set;}
    public int X {get; set;}

    public override string? ToString()
    {
        return Id?.Replace("x", X.ToString());
    }
}

public class Weapon
{
    /// <summary>
    /// Weapon name
    /// </summary>
    public string? Name {get; set;}
    /// <summary>
    /// Weapon kind
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WeaponKind Kind {get; set;}
    /// <summary>
    /// Number of attack dice
    /// </summary>
    public int Atk {get; set;}
    /// <summary>
    /// Min number on the dice to be considered a hit
    /// </summary>
    public int Hit {get; set;}
    /// <summary>
    /// Amount of damage
    /// </summary>
    public Damage? Damage {get; set;}
    /// <summary>
    /// Weapon rules
    /// </summary>
    public List<WeaponRule>? Rules {get; set;}
}