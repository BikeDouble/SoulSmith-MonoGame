
using System;
using System.Collections.Generic;
using SoulSmithStats;

public partial class CampBuilding_Fuser : CampBuilding
{
	private double _statRandRange = 0.2f;

	public CampBuilding_Fuser(int inputSlots) : base(inputSlots)
	{

	}

	private Unit FuseUnits(List<Unit> units)
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
		
		//TODO finish function
		return null;
	}

	private int CalculateStatOutcome(int unitCount, int stat)
	{
		double coef = Rand.RandDoubleAroundOne(_statRandRange);
		int result = (int)(coef * stat);
		double countCoef = 1f + (double)((unitCount - 1) / unitCount);
		result = (int)(result * countCoef);
		return result;
	}
}
