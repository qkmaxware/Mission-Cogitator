using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kt.Data;

public class RuleDatabase
{
    private Dictionary<string, Rule> rules;
    private Dictionary<string, Effect> effects;
    private Dictionary<string, Equipment> equipment;

    public IEnumerable<Effect> AllEffects => effects.Values;
    public IEnumerable<Rule> AllRules => rules.Values;
    public IEnumerable<Equipment> AllEquipment => equipment.Values;

    public RuleDatabase()
    {
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

    public Rule[] GetCoreActions()
    {
        return [
            this.GetValue(id: "reposition")!,
            this.GetValue(id: "charge")!,
            this.GetValue(id: "fall-back")!,
            this.GetValue(id: "dash")!,
            this.GetValue(id: "shoot")!,
            this.GetValue(id: "fight")!,
            this.GetValue(id: "pick-up-marker")!,
          //this.GetValue(id: "place-marker")!,
            this.GetValue(id: "counteract")!
        ];
    }

    public Rule[] GetCoreWeaponRules()
    {
        return [
            this.GetValue(id: "accurate")!,
            this.GetValue(id: "balanced")!,
            this.GetValue(id: "blast")!,
            this.GetValue(id: "brutal")!,
            this.GetValue(id: "ceaseless")!,
            this.GetValue(id: "devastating")!,
            this.GetValue(id: "heavy")!,
            this.GetValue(id: "hot")!,
            this.GetValue(id: "lethal")!,
            this.GetValue(id: "limited")!,
            this.GetValue(id: "piercing")!,
            this.GetValue(id: "punishing")!,
            this.GetValue(id: "range")!,
            this.GetValue(id: "relentless")!,
            this.GetValue(id: "rending")!,
            this.GetValue(id: "saturate")!,
            this.GetValue(id: "seek")!,
            this.GetValue(id: "severe")!,
            this.GetValue(id: "shock")!,
            this.GetValue(id: "silent")!,
            this.GetValue(id: "stun")!,
            this.GetValue(id: "torrent")!,
        ];
    }

    public IEnumerable<Equipment> GetUniversalEquipment()
    {
        yield return (Equipment)this.GetValue(id: "ammo-cache")!;
        yield return (Equipment)this.GetValue(id: "breaching-charge")!;
        yield return (Equipment)this.GetValue(id: "comms-device")!;
        yield return (Equipment)this.GetValue(id: "frag")!;
        yield return (Equipment)this.GetValue(id: "heavy-barricade")!;
        yield return (Equipment)this.GetValue(id: "krak")!;
        yield return (Equipment)this.GetValue(id: "ladders")!;
        yield return (Equipment)this.GetValue(id: "light-barricades")!;
        yield return (Equipment)this.GetValue(id: "mines")!;
        yield return (Equipment)this.GetValue(id: "portable-barricade")!;
        yield return (Equipment)this.GetValue(id: "razor-wire")!;
        yield return (Equipment)this.GetValue(id: "smoke-utility-grenades")!;
        yield return (Equipment)this.GetValue(id: "stun-utility-grenades")!;
    }

    public void Alias(string uid, string @as)
    {
        if (!rules.TryGetValue(uid, out Rule? rule))
            return;
        
        if (rules.ContainsKey(@as))
            return; // Don't replace existing stuff for an ALIAS

        rules[@as] = rule;
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