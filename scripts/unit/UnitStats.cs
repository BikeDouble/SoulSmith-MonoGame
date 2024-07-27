using System;
using SoulSmithMoves;
using SoulSmithStats;
using System.Collections.Generic;

public partial class UnitStats 
{
	private StatsList _statsList;

	private int _combatPosition;
	private List<Modifier> _modifiers = new List<Modifier>();

	public UnitStats()
	{
		_statsList = new StatsList();
    }

	public UnitStats(StatsList statsList)
	{
		_statsList = statsList;
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
        StatModifier combinedModifier = CombineStatModifiers(statModifiers);
        int retValue = ApplyStatModifier(value, combinedModifier);

		return retValue;
    }

    private List<StatModifier> GetRelevantStatModifiers(StatType stat)
    {
        List<StatModifier> modifiers = new List<StatModifier>();
        foreach (Modifier modifier in _modifiers)
        {
            StatModifier statModifier = modifier.GetStatModifier(stat);
            if (statModifier.Stats.Contains(stat))
            {
                modifiers.Add(statModifier);
            }
        }

		return modifiers;
    }

    private StatModifier CombineStatModifiers(List<StatModifier> statModifiers)
    {
        if (statModifiers.Count == 0)
        {
            return new StatModifier();
        }

        StatType stat = StatType.None;
        int flatMod = 0;
        double additiveMod = 0f;
        double multiplicativeMod = 1f;

        foreach (StatModifier statModifier in statModifiers)
        {
            flatMod += statModifier.FlatMod;
            additiveMod += statModifier.AdditiveMod;
            multiplicativeMod *= statModifier.MultiplicativeMod;

        }

        return new StatModifier(stat, flatMod, additiveMod, multiplicativeMod);
    }

	private int ApplyStatModifier(int baseStat, StatModifier modifier)
	{
		int retStat = baseStat + modifier.FlatMod;
		retStat = (int)(retStat * (modifier.AdditiveMod + 1));
		retStat = (int)(retStat * modifier.MultiplicativeMod);

		return retStat;
	}

    //
    // Modifier related functions
    //

	private void AddModifier(Modifier modifier)
	{
        _modifiers.Add(modifier);
		modifier.RemoveModifierEventHandler += RemoveModifier;
		modifier.EnqueueEffectInputEventHandler += EnqueueEffectInput;
        modifier.OnApplied();
    }

	private void RemoveModifier(object sender, RemoveModifierEventArgs e)
	{
		Modifier modifier = e.Modifier;
		_modifiers.Remove(modifier);
	}

	//
	// Damage related functions
	//

	// Returns amount of HP actually lost
	private int LoseHP(int damage, bool gainDecay = true)
	{
		int newHealth = GetBaseStat(StatType.CurHealth) - damage;

        SetStat(StatType.CurHealth, newHealth);

		int effectiveDamage = damage;

        if (newHealth <= 0)
        {
			effectiveDamage += newHealth;
        }

        if (gainDecay)
		{
			int newDecay = GetBaseStat(StatType.CurDecay) + ((effectiveDamage * GetModStat(StatType.DecayRate))/100);
			SetStat(StatType.CurDecay, newDecay);
		}

		int decayedMaxHealth = GetBaseStat(StatType.MaxHealth) - GetBaseStat(StatType.CurDecay);


        if (newHealth > decayedMaxHealth)
		{
			newHealth = decayedMaxHealth;
			SetStat(StatType.CurHealth, newHealth);
		}

		if (newHealth <= 0)
		{
			CallForDeath();
        }
		
		UpdateUI();

		return effectiveDamage;
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

	public event EventHandler<UpdateUIEventArgs> UpdateUIEventHandler;

	private void UpdateUI()
	{
		UpdateUIEventArgs e = new UpdateUIEventArgs();

		UpdateUIEventHandler(this, e);
	}

	public event EventHandler<ZeroHPEventArgs> ZeroHPEventHandler;

	private void CallForDeath()
	{
		ZeroHPEventArgs e = new ZeroHPEventArgs();

		ZeroHPEventHandler(this, e);
	}

	public event EventHandler<SendEffectEventArgs> SendEffectEventHandler;

	//Sends an effect request up the tree, called by effect queue
	public void SendEffect(EffectRequest request)
	{
		SendEffectEventArgs e = new SendEffectEventArgs();
		e.EffectRequest = request;

		SendEffectEventHandler(this, e);
	}

	//
	// Healing related functions
	//

	// Returns amount of hp actually gained
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

		UpdateUI();

		return effectiveHealing;
    }

	public void OnDeath()
	{
        SetStat(StatType.CurHealth, 0);
        SetStat(StatType.CurDecay, GetBaseStat(StatType.MaxHealth));
		UpdateUI();
    }

	//
	// Triggers
	//

	public void OnTrigger(EffectTrigger trigger, Unit sender)
	{
		switch (trigger)
		{
			case EffectTrigger.OnHitting:
				OnHitting(sender);
				break;
			case EffectTrigger.OnBeingHit:
				OnBeingHit(sender);
				break;
			case EffectTrigger.OnMoveBegin:
				OnMoveBegin(sender); 
				break;
			case EffectTrigger.OnMoveEnd: 
				OnMoveEnd(sender);
				break;
			case EffectTrigger.OnTurnBegin:
				OnTurnBegin(sender);
				break;
			case EffectTrigger.OnTurnEnd:
				OnTurnEnd(sender);
				break;
			case EffectTrigger.OnRoundBegin:
				OnRoundBegin(sender);
				break;
			case EffectTrigger.OnRoundEnd:
				OnRoundEnd(sender);
				break;
			default:
				break;
		}
	}

	private void OnHitting(Unit target)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnHitting(target);
		}
	}

	private void OnBeingHit(Unit hitter)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnBeingHit(hitter);
		}
	}

	private void OnMoveBegin(Unit target)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnMoveBegin(target);
		}
	}

	private void OnMoveEnd(Unit target)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnMoveEnd(target);
		}
	}

	private void OnTurnBegin(Unit self)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnTurnBegin(self);
		}
	}

	private void OnTurnEnd(Unit mover)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnTurnEnd(mover);
		}
	}

    private void OnRoundBegin(Unit self)
    {
        foreach (Modifier modifier in _modifiers)
        {
            modifier.OnRoundBegin(self);
        }
    }

    private void OnRoundEnd(Unit self)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnRoundEnd(self);
		}
	}

    public EffectResult ExecuteEffect(EffectRequest request)
    {
		if (request == null)
		{
			return new EffectResult();
		}

		if (request.RawDamage != 0)
		{
			return ExecuteDamageEffect(request);
		}

        if (request.RawHealing != 0)
        {
            return ExecuteHealingEffect(request);
        }

        if (request.Modifier != null)
		{
			return ExecuteModifierEffect(request);
		}

		if (request.Trigger != EffectTrigger.None)
		{
			return ExecuteTriggerEffect(request);
		}

		return new EffectResult();
    }

	private EffectResult ExecuteDamageEffect(EffectRequest request)
	{
		int hpLoss;
        DamageType damageType = request.DamageType;
		EffectResult effectResult = new EffectResult();

        switch (damageType)
        {
            case DamageType.Hit:
				hpLoss = StandardDefenseCalculation(request.RawDamage, GetModStat(StatType.Defense));
                break;
			case DamageType.Essence:
                hpLoss = StandardDefenseCalculation(request.RawDamage, GetModStat(StatType.Defense));
                break;
            default:
				hpLoss = 0;
				break;
        }

		effectResult.EffectiveDamage = LoseHP(hpLoss, request.GainDecay);

		return effectResult;
    }

	private EffectResult ExecuteHealingEffect(EffectRequest request)
	{
		EffectResult result = new EffectResult();

		int rawHealing = request.RawHealing;
		int effectiveHealing = GainHP(rawHealing);
		result.EffectiveHealing = effectiveHealing;

		return result;
	}

	private EffectResult ExecuteModifierEffect(EffectRequest effectResult)
	{
		EffectResult result = new EffectResult();

		Modifier modifier = effectResult.Modifier;
		modifier.ApplyTo(effectResult.Target, effectResult.Sender);
		AddModifier(modifier);
		result.ModifierApplied = true;

		return result;
	}

	private EffectResult ExecuteTriggerEffect(EffectRequest effectResult)
	{
        EffectResult result = new EffectResult();

        OnTrigger(effectResult.Trigger, effectResult.Sender);
		result.TriggerApplied = true;

		return result;
	}

    public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
}

public class SendEffectEventArgs : EventArgs
{
	public EffectRequest EffectRequest;
}

public class UpdateUIEventArgs : EventArgs
{

}

public class ZeroHPEventArgs : EventArgs
{
	public Unit Unit;
}