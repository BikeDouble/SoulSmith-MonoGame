
using System;
using System.Collections.Generic;
using SoulSmith.UnitStats;
using SoulSmith.Units;
using SoulSmith.Core;
using SoulSmith.EmotionTags;
using SoulSmith.Collections;
using SoulSmith.Battle.Emotions;
using SoulSmith.Templates;

namespace SoulSmith.Camps;
public partial class CampBuilding_Fuser : CampBuilding
{
	public const double STATRANDRANGE = 0.2f;
	public const double STATCOEF = 1.3333f;

	public CampBuilding_Fuser(int inputSlotCount, int outputSlotCount) : base(inputSlotCount, outputSlotCount)
	{

	}

	public override void Complete()
	{
		base.Complete();

        // Check that we have enough output space
		if (!(OutputUnits.Count >= OutputCapacity))
		{
            Unit newUnit = FuseUnits(InputUnits);
			if (newUnit != null)
			{
				OutputUnit(newUnit);
				DisposeInputUnits();
            } else {
				throw new InvalidOperationException("Fusing units failed, resulting unit is null.");
            }
        }
	}

	private Unit FuseUnits(IReadOnlyList<Unit> units)
	{
		if (units.Count == 1)
		{
			return units[0];
		}

		if (units.Count < 1)
		{
			return null;
		}

		int healthSum = 0;
		int atkSum = 0;
		int defSum = 0;
		foreach (Unit unit in units)
		{
			if (unit != null)
			{
				healthSum += unit.GetBaseStat(StatType.MaxHealth);
				atkSum += unit.GetBaseStat(StatType.Attack);
				defSum += unit.GetBaseStat(StatType.Defense);
			}
		}

		healthSum = CalculateStatOutcome(units.Count, healthSum);
		atkSum = CalculateStatOutcome(units.Count, atkSum);
		defSum = CalculateStatOutcome(units.Count, defSum);

		int newHealth = healthSum / units.Count;
		int newAtk = atkSum / units.Count;
		int newDef = defSum / units.Count;

		EmotionTag newTag = CalculateFusedEmotionTag(units);

		UnitTemplate newTemplate = Emotion.GetEmotion(newTag).UnitTemplate.GetModdedCopy(new Dictionary<StatType, int>()
		{
			{ StatType.MaxHealth, newHealth },
			{ StatType.Attack, newAtk },
			{ StatType.Defense, newDef }
		});

        return new Unit(newTemplate); //TODO test
	}

	private int CalculateStatOutcome(int unitCount, int stat)
	{
		double coef = Rand.RandDoubleAroundOne(STATRANDRANGE);
		int result = (int)(coef * stat);
		double countCoef = 1f + (double)((unitCount - 1) / unitCount);
		result = (int)(result * countCoef);
		return result;
	}

    private EmotionTag CalculateFusedEmotionTag(IEnumerable<Unit> units)
    {
        var counts = new Dictionary<EmotionTag, int>();

        if (units != null)
        {
            foreach (Unit u in units)
            {
                if (u == null)
                    continue;

                foreach (EmotionTag baseT in Emotion.GetBaseEmotionTags(u.EmotionTag))
                {
                    if (counts.TryGetValue(baseT, out int current))
                    {
                        counts[baseT] = current + 1;
                    }
                    else
                    {
                        counts[baseT] = 1;
                    }
                }
            }
        }

        SoulSmithWeightedList<EmotionTag> tagList = new SoulSmithWeightedList<EmotionTag>(null, Rand.Random);

        foreach (KeyValuePair<EmotionTag, int> kvp in counts)
        {
            tagList.Add(kvp.Key, kvp.Value);
        }

        if (tagList.Count == 0)
        {
            return EmotionTag.Typeless;
        }

		EmotionTag ret = EmotionTag.Typeless;

		foreach (EmotionTag t in tagList.NextMultiple(3)) 
		{
			ret = Emotion.CombineTags(ret, t);
		}

        return ret;
    }
}
