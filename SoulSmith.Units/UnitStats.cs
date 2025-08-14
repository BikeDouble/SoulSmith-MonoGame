using SoulSmith.UnitStats;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SoulSmith.Battle;
using SoulSmith.Object;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Units;
public class UnitStats : SoulSmithObject, IReadOnlyUnitStats
{
    public const float NATURALDECAYFROMDAMAGERATE = 0.33f;

    private StatsList _statsList;

	private int _combatPosition;
	private List<IModifier> _modifiers = new List<IModifier>();
	private int _timeOnBoard = -1;
	private IEffect _naturalDamageDecayEffect = new NaturalDamageDecayEffect(NATURALDECAYFROMDAMAGERATE);

	public UnitStats()
	{
		_statsList = new StatsList();
    }

	public UnitStats(StatsList statsList, int timeOnBoard = -1, EmotionTag.EmotionTag emotion = EmotionTag.EmotionTag.Typeless)
	{
		_statsList = statsList;
		_timeOnBoard = timeOnBoard;
	}

	public void LoadEmotionAttributes(EmotionTag.EmotionTag emotionTag)
	{
		EmotionTag.EmotionTag emotion = emotionTag;

		if (emotion == null)
		{
			Trace.TraceError("Null emotion with tag: " + emotionTag);
			return;
		}
		
		/*ReadOnlyCollection<ModifierTemplateWithArgs> modifierTemplates = emotion.PermanentModifiers; //TODO make emotion globally accessible with emotion tag
		if (modifierTemplates != null)
		{
			foreach (ModifierTemplateWithArgs modifierTemplate in modifierTemplates)
			{
                ApplyModifier(modifierTemplate, (IReadOnlyUnit)GetParent());
            }
		}*/
	}

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    private void EnqueueEffectInput(object sender, EnqueueEffectInputEventArgs e)
    {
		EnqueueEffectInputEventHandler(this, e);
    }

    public int GetBaseStat(StatType statType)
	{
		return _statsList[statType];
	}

	public void SetStat(StatType statType, int newValue)
	{
		_statsList[statType] = newValue;
    }

    /// <summary>
    /// Returns the specified stats with all necessary modifiers applied
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public int GetModStat(StatType statType)
	{
		int retStat = GetBaseStat(statType);
        retStat = ApplyAllRelevantStatModifiers(retStat, statType);
		return retStat;
	}

	private int ApplyAllRelevantStatModifiers(int value, StatType stat) 
	{
        List<StatModifier> statModifiers = GetRelevantStatModifiers(stat);
        (float Flat, float AdditivePercent, float MultiplicativePercent) combinedModifier = CombineStatModifiers(statModifiers);
        int retValue = ApplyStatModifier(value, combinedModifier);

		return retValue;
    }

    private List<StatModifier> GetRelevantStatModifiers(StatType stat)
    {
        List<StatModifier> modifiers = new List<StatModifier>();
        foreach (IModifier modifier in _modifiers)
        {
            StatModifier? statModifier = modifier.GetStatModifier();
			if (statModifier.HasValue)
			{
				if (statModifier.Value.Stat == stat)
				{
					modifiers.Add(statModifier.Value);
				}
			}
        }

		return modifiers;
    }

    private (float Flat, float AdditivePercent, float MultiplicativePercent) CombineStatModifiers(List<StatModifier> statModifiers)
    {
        if (statModifiers.Count == 0)
        {
            return (0, 0, 1);
        }

        float flat = 0;
        float additivePercent = 0f;
        float multiplicativePercent = 1f;

        foreach (StatModifier statModifier in statModifiers)
        {
			switch (statModifier.ModStyle)
			{
				case StatModStyle.Flat:
					flat = StatTypeHelper.CombineModifiers(flat, statModifier.ModAmount, StatModStyle.Flat);
					break;
				case StatModStyle.AdditivePercent:
					additivePercent = StatTypeHelper.CombineModifiers(additivePercent, statModifier.ModAmount, StatModStyle.AdditivePercent);
					break;
				case StatModStyle.MultiplicativePercent:
                    multiplicativePercent = StatTypeHelper.CombineModifiers(multiplicativePercent, statModifier.ModAmount, StatModStyle.MultiplicativePercent);
					break;

            }
        }

		return (flat, additivePercent, multiplicativePercent);
    }

	private int ApplyStatModifier(int baseStat, (double Flat, double AdditivePercent, double MultiplicativePercent) modifierResults)
	{
		// Flat modifier is applied immediately to the base stat.
		int retStat = baseStat + (int)modifierResults.Flat;

		// Additive percent modifier is applied to the base stat after the flat modifier.
		float additiveAsDecimal = (float)((modifierResults.AdditivePercent + 100) / 100);
		retStat = (int)(retStat * additiveAsDecimal);

		// Multiplicative percent modifier is applied to the base stat after the flat and additive percent modifiers.
        float multiplicativeAsDecimal = (float)((modifierResults.MultiplicativePercent + 100) / 100);
        retStat = (int)(retStat * multiplicativeAsDecimal);

		return retStat;
	}

    //
    // Modifier related functions
    //

	public event EventHandler<ModifierAddOrRemoveEventArgs> ModifierAddEventHandler;

	/// <summary>
	/// Adds modifier to list and links events to this UnitStats object
	/// </summary>
	/// <param name="modifier"></param>
	private void AddModifier(IModifier modifier)
	{
        _modifiers.Add(modifier);
		modifier.EnqueueEffectInputEventHandler += EnqueueEffectInput;

		ModifierAddOrRemoveEventArgs e = new();
		e.Modifier = modifier;

		ModifierAddEventHandler?.Invoke(this, e);
    }

	private List<IModifier> _modifiersToBeRemoved = new List<IModifier>();

	public event EventHandler<ModifierAddOrRemoveEventArgs> ModifierRemoveEventHandler;

	private void RemoveModifier(object sender, RemoveModifierEventArgs e)
	{
		IModifier modifier = e.Modifier;
        modifier.EnqueueEffectInputEventHandler -= EnqueueEffectInput;
		_modifiersToBeRemoved.Add(modifier);

        ModifierAddOrRemoveEventArgs e2 = new();
        e2.Modifier = modifier;

        ModifierRemoveEventHandler?.Invoke(this, e2);
    }

	private void ClearModifiersToBeRemovedList()
	{
		foreach (IModifier modifier in _modifiersToBeRemoved)
		{
			_modifiers.Remove(modifier);
		}

		_modifiersToBeRemoved.Clear();
	}

	//
	// Damage related functions
	//

	// Returns amount of HP actually lost
	private static int CalculateEffectiveDamage(int damage, IReadOnlyUnitStats stats)
	{
		int newHealth = stats.GetBaseStat(StatType.CurHealth) - damage;

		int effectiveDamage = damage;

        if (newHealth < 0)
        {
			effectiveDamage += newHealth;
        }

		return effectiveDamage;
	}

	private static int CalculateEffectiveDecay(int decay, IReadOnlyUnitStats stats)
	{
		int newDecay = stats.GetBaseStat(StatType.CurDecay) + decay;

		int effectiveDecay = decay;

		if (newDecay > stats.GetBaseStat(StatType.MaxHealth))
		{
			effectiveDecay -= (newDecay - stats.GetBaseStat(StatType.MaxHealth));
		}

		return effectiveDecay;
    }

    private int StandardDefenseCalculation(int damage, int defense)
    {
        double coef = (double)100 / (double)defense;
        int ret = (int)(damage * coef);
        if (ret <= 0)
        {
            ret = 1;
        }
        return ret;
    }

    //
    // Signal emitters
    //

	public event EventHandler<UnitDeathCallArgs> UnitDeathCallEventHandler;

	private void CallForDeath(IReadOnlyUnit killer, Result killingEffectResult)
	{
		SetDeathStats();

		UnitDeathCallArgs e = new UnitDeathCallArgs();

		e.Killer = killer;
		e.KillingEffectResult = killingEffectResult;

        UnitDeathCallEventHandler(this, e);
    }

    //
    // Healing related functions
    //

    // Returns amount of hp actually gained, and sets the current health stat to the new value
    private int GainHP(int healing)
	{
		int newHealth = GetBaseStat(StatType.CurHealth + healing);
		SetStat(StatType.CurHealth, newHealth);

        int effectiveHealing = healing;

        int decayedMaxHealth = GetBaseStat(StatType.MaxHealth) - GetBaseStat(StatType.CurDecay);

		if (newHealth > decayedMaxHealth)
		{
			int lostHealing = newHealth - decayedMaxHealth;

			effectiveHealing -= lostHealing;

			newHealth = decayedMaxHealth;
            SetStat(StatType.CurHealth, newHealth);
        }

		return effectiveHealing;
    }

	//
	// Triggers
	//

    public Result ExecutePayload(Payload payload)
    {
		if (payload == null)
		{
			return null;
		}

		Result result = null;

		switch (payload)
		{
			case DamagePayload damagePayload:
				result = ExecuteDamagePayload(damagePayload);
				break;
			case DecayPayload decayPayload:
				result = ExecuteDecayPayload(decayPayload);
				break;
			case HealingPayload healingPayload:
				result = ExecuteHealingPayload(healingPayload);
				break;
			case AddModifierPayload addModifierPayload:
				result = ExecuteAddModifierPayload(addModifierPayload);
				break;
			case RemoveModifierPayload removeModifierPayload:
				result = ExecuteRemoveModifierPayload(removeModifierPayload);
				break;
            default:
				throw new NotImplementedException($"Payload type {payload.GetType()} is not implemented in UnitStats.ExecutePayload.");
        }
		
		return result;
    }

	public void ReactToPayloadResult(Result result)
	{
		switch(result)
		{
			case TriggerResult triggerResult:
				if (triggerResult.Trigger == CombatTrigger.OnRoundEnd)
					if (_timeOnBoard > -1) 
						DecrementTimeOnBoard();
                break;
			case DamageResult damageResult:
                Unit parent = (Unit)this.GetParent(); //TODO type safety
                if (damageResult.Target == parent)
                {
                    if (damageResult.EffectiveDamage > 0)
                    {
                        EnqueueEffectInputEventArgs e = new();
                        EffectInput effectInput = new EffectInput(_naturalDamageDecayEffect, parent, parent, Priority.NaturalDecayDamage, this, result);
                        e.EffectInput = effectInput;
                        EnqueueEffectInput(this, e);
                    }
                }
				break;
        }

		foreach (IModifier modifier in _modifiers) 
		{
			modifier.ReactToPayloadResult(result);
		}

        ClearModifiersToBeRemovedList();
	}

	public void ModifyPayload(Payload payload)
	{
		foreach (IModifier modifier in _modifiers)
		{
			modifier.ModifyPayload(payload);
		}

        ClearModifiersToBeRemovedList();
    }

	private void DecrementTimeOnBoard()
	{
		_timeOnBoard--;

		if (_timeOnBoard <= 0)
		{
			CallForDeath(null, null); //TODO make this retreat instead
		}
	}

	private Result ExecuteDamagePayload(DamagePayload payload)
	{
		int hpLoss = 0;
        DamageType damageType = payload.DamageType;

        switch (damageType)
        {
            case DamageType.Hit:
				hpLoss = StandardDefenseCalculation(payload.RawDamage, GetModStat(StatType.Defense));
                break;
			case DamageType.Essence:
                hpLoss = StandardDefenseCalculation(payload.RawDamage, GetModStat(StatType.Defense));
                break;
            default:
				throw new ArgumentException($"Unknown damage type: {damageType}");
        }

		int effectiveDamage = CalculateEffectiveDamage(hpLoss, this);

		if (effectiveDamage > 0)
		{
			int newHP = GetModStat(StatType.CurHealth) - effectiveDamage;
			SetStat(StatType.CurHealth, newHP);
		}

		bool killed = GetModStat(StatType.CurHealth) <= 0;

		Result result = new DamageResult(payload.Sender, payload.Target, effectiveDamage, damageType, killed, payload.ParentResult, payload, payload.Originator);

		if (killed) CallForDeath(payload.Sender, result);

        return result;
    }

	private Result ExecuteDecayPayload(DecayPayload payload)
	{
		int decayGain = payload.RawDecay;

		int ratedDecayGain = decayGain * GetModStat(StatType.DecayRate) / 100;

		int effectiveDecay = CalculateEffectiveDecay(ratedDecayGain, this);

        if (effectiveDecay > 0)
        {
            int newDecay = GetBaseStat(StatType.CurDecay) + effectiveDecay;
            SetStat(StatType.CurDecay, newDecay);
        }

        Result effectResult = new DecayResult(payload.Sender, payload.Target, effectiveDecay, payload.ParentResult, payload, payload.Originator);

        int undecayedHealthRoom = GetModStat(StatType.MaxHealth) - GetModStat(StatType.CurDecay);

		if (GetModStat(StatType.CurHealth) > undecayedHealthRoom)
		{
			SetStat(StatType.CurHealth, undecayedHealthRoom);
            if (GetModStat(StatType.CurHealth) <= 0) CallForDeath(payload.Sender, effectResult);
        }

		return effectResult;
    }

	private Result ExecuteHealingPayload(HealingPayload payload)
	{
		int rawHealing = payload.RawHealing;

		if (rawHealing == 0) return null;

		int effectiveHealing = GainHP(rawHealing);
		Result result = new HealingResult(payload.Sender, payload.Target, effectiveHealing, payload.ParentResult, payload, payload.Originator);

		return result;
	}

	private Result ExecuteAddModifierPayload(AddModifierPayload payload)
	{
		if (payload.Modifier == null) throw new ArgumentNullException("Modifier cannot be null.");

		ApplyModifier(payload.Modifier, payload.Sender);

		Result result = new AddModifierResult(payload.Sender, payload.Target, payload.Modifier, true, payload.ParentResult, payload, payload.Originator);

        return result;
	}

    private Result ExecuteRemoveModifierPayload(RemoveModifierPayload payload)
    {
        if (payload.Modifier == null) throw new ArgumentException("EnqueueRemove modifierResults payload does not contain a modifierResults");

		if (!_modifiers.Contains(payload.Modifier)) return new RemoveModifierResult(payload.Sender, payload.Target, payload.Modifier, false, payload.ParentResult, payload, payload.Originator); //Modifier may have been removed already

		RemoveModifierEventArgs e = new RemoveModifierEventArgs();
		e.Modifier = payload.Modifier;

		RemoveModifier(this, e);

        Result result = new RemoveModifierResult(payload.Sender, payload.Target, payload.Modifier, true, payload.ParentResult, payload, payload.Originator);

        return result;
    }

	private void ClearModifiers() 
	{
		foreach (IModifier modifier in _modifiers)
		{
            RemoveModifierEventArgs e = new RemoveModifierEventArgs();
            e.Modifier = modifier;

            RemoveModifier(this, e);
        }
	}

    private void ApplyModifier(IModifier modifier, IReadOnlyUnit sender)
	{
		if (modifier == null) return;

		bool merged = false;

        foreach (IModifier mergeCandidate in _modifiers)
		{
			if (mergeCandidate.TryMerge(modifier)) 
			{
				merged = true;
                break;
			}
		}

		if (!merged)
        {
            AddModifier(modifier);
            modifier.ApplyModifier(sender, (IReadOnlyUnit)this.GetParent());
        }
    }

	public IReadOnlyModifier GetReadOnlyModifier(string mergeKey)
	{
		foreach (IModifier modifier in _modifiers)
        {
            if (modifier.MergeKey == mergeKey)
                return modifier;
        }

		return null;
	}

	public void OnRetreat()
	{
        ClearModifiers();
    }

	public void OnDeath()
	{
		SetDeathStats();
        ClearModifiers();
    }

	private void SetDeathStats()
	{
        SetStat(StatType.CurHealth, 0);
        SetStat(StatType.CurDecay, GetModStat(StatType.MaxHealth));
    }

	public ReadOnlyDictionary<StatType, int> StatsList { get { return _statsList.StatsDict; } }
    public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
	public int TimeOnBoard { get { return _timeOnBoard; } }
	public int MaxHealth { get { return GetModStat(StatType.MaxHealth); } }
    public int CurHealth { get { return GetModStat(StatType.CurHealth); } }
    public int Attack { get { return GetModStat(StatType.Attack); } }
    public int Defense { get { return GetModStat(StatType.Defense); } }
    public int DecayRate { get { return GetModStat(StatType.DecayRate); } }
    public int CurDecay { get { return GetModStat(StatType.CurDecay); } }
}

public class UpdateUIEventArgs : EventArgs
{

}

public class UnitDeathCallArgs : EventArgs
{
	public IReadOnlyUnit CallingUnit;
	public IReadOnlyUnit Killer;
	public Result KillingEffectResult;
}

public class ModifierAddOrRemoveEventArgs
{
	public IModifier Modifier;
}