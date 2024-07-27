
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

public static class TypelessMoveTemplates
{
    // Hit
    private const string TYPELESSHITFRIENDLYNAME = "Hit";
    private const string TYPELESSHITFRIENDLYDESCRIPT = "Hit an enemy for 100% of your attack stat.";
    private const string TYPELESSHITTEMPLATENAME = "TypelessHit";

    // Double Hit
    private const string TYPLESSDOUBLEHITFRIENDLYNAME = "Double Hit";
    private const string TYPLESSDOUBLEHITFRIENDLYDESCRIPT = "Hit an enemy for 40% of your attack stat twice.";
    private const string TYPELESSDOUBLEHITTEMPLATENAME = "TypelessDoubleHit";
    
    // Attack Up
    private const string ATTACKUPFRIENDLYNAME = "Attack Up";
    private const string ATTACKUPFRIENDLYDESCRIPT = "Boost the attack of any unit on your team.";
    private const string ATTACKUPTEMPLATENAME = "TypelessAttackUp";

    // Visualization names
    private const string TYPELESSPELLETVISUALIZATIONNAME = "TypelessPellet";

    public static Dictionary<string, MoveTemplate> CreateDict()
    {
        List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

        dicts.Add(BasicMoves());

        return MoveTemplateLibrary.MergeDictionaries(dicts);
    }

    private static Dictionary<string, MoveTemplate> BasicMoves()
	{
        Dictionary<string, MoveTemplate> dict = new();

		dict.TryAdd(TYPELESSHITTEMPLATENAME, Hit());
        dict.TryAdd(TYPELESSDOUBLEHITTEMPLATENAME, DoubleHit());
        dict.TryAdd(ATTACKUPTEMPLATENAME, AttackUp());

        return dict;
	}

    private static MoveTemplate Hit()
    {
        List<Effect> effects = new List<Effect>();
        Effect effect = MoveHelpers.AttackHit(1f, TYPELESSPELLETVISUALIZATIONNAME);
        effects.Add(effect);

        MoveTemplate move = new MoveTemplate(TYPELESSHITFRIENDLYNAME, TYPELESSHITFRIENDLYDESCRIPT, effects);

        return move;
    }

    private static MoveTemplate DoubleHit()
    {

        List<Effect> effects = new List<Effect>();
        Effect effect = MoveHelpers.AttackHit(0.4f, TYPELESSPELLETVISUALIZATIONNAME);
        effects.Add(effect);

        effect = MoveHelpers.AttackHit(0.4f, TYPELESSPELLETVISUALIZATIONNAME, 0.3f);
        effects.Add(effect);

        MoveTemplate move = new MoveTemplate(TYPLESSDOUBLEHITFRIENDLYNAME, TYPLESSDOUBLEHITFRIENDLYDESCRIPT, effects);
        return move;
    }

    private static MoveTemplate AttackUp()
    {
        List<Effect> effects = new List<Effect>();
        StatModifier statModifier = new StatModifier(StatType.Attack, 0, 4, 1);
        Effect effect = new Effect_StatModifier(statModifier, EffectTargetingStyle.MoveTarget);
        effects.Add(effect);

        MoveTemplate move = new MoveTemplate(ATTACKUPFRIENDLYNAME, ATTACKUPFRIENDLYDESCRIPT, effects, MoveTargetingStyle.AllyOrSelf, EmotionTag.Typeless);

        return move;
    }
}
