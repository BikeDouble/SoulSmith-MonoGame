using System;
using SoulSmithModifiers;
using SoulSmithMoves;
using SoulSmithStats;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SoulSmithEmotions;

public partial class UnitStats : SoulSmithObject, IReadOnlyUnitStats
{
	private StatsList _statsList;

	private int _combatPosition;
	private List<Modifier> _modifiers = new List<Modifier>();
	private int _timeOnBoard = -1;

	public UnitStats()
	{
		_statsList = new StatsList();
    }

	public UnitStats(StatsList statsList, int timeOnBoard = -1, EmotionTag emotion = EmotionTag.Typeless)
	{
		_statsList = statsList;
		_timeOnBoard = timeOnBoard;
	}

	public void LoadEmotionAttributes(EmotionTag emotionTag)
	{
		Emotion emotion = AssetLoader.GetEmotion(emotionTag);

		if (emotion == null)
		{
			Trace.TraceError("Null emotion with tag: " + emotionTag);
			return;
		}

		ReadOnlyCollection<ModifierTemplateWithArgs> modifierTemplates = emotion.PermanentModifiers;
		if (modifierTemplates != null)
		{
			foreach (ModifierTemplateWithArgs modifierTemplate in modifierTemplates)
			{
                ApplyModifier(modifierTemplate, (IReadOnlyUnit)GetParent());
            }
		}
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
            if (statModifier.Stat == stat)
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

	public event EventHandler<ModifierAddOrRemoveEventArgs> ModifierAddEventHandler;

	private void AddModifier(Modifier modifier)
	{
        _modifiers.Add(modifier);
		modifier.RemoveModifierEventHandler += RemoveModifier;
		modifier.EnqueueEffectInputEventHandler += EnqueueEffectInput;

		ModifierAddOrRemoveEventArgs e = new();
		e.Modifier = modifier;

		ModifierAddEventHandler?.Invoke(this, e);
    }

	private List<Modifier> _modifiersToBeRemoved = new List<Modifier>();

	public event EventHandler<ModifierAddOrRemoveEventArgs> ModifierRemoveEventHandler;

	private void RemoveModifier(object sender, RemoveModifierEventArgs e)
	{
		Modifier modifier = e.Modifier;
        modifier.RemoveModifierEventHandler -= RemoveModifier;
        modifier.EnqueueEffectInputEventHandler -= EnqueueEffectInput;
		_modifiersToBeRemoved.Add(modifier);

        ModifierAddOrRemoveEventArgs e2 = new();
        e2.Modifier = modifier;

        ModifierRemoveEventHandler?.Invoke(this, e2);
    }

	private void ClearModifiersToBeRemovedList()
	{
		foreach (Modifier modifier in _modifiersToBeRemoved)
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

        if (newHealth <= 0)
        {
			effectiveDamage += newHealth;
        }

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

	public event EventHandler<UnitDeathCallArgs> UnitDeathCallEventHandler;

	private void CallForDeath(IReadOnlyUnit killer)
	{
		UnitDeathCallArgs e = new UnitDeathCallArgs();

		e.Killer = killer;

		UnitDeathCallEventHandler(this, e);
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

		return effectiveHealing;
    }

	//
	// Triggers
	//

    public EffectResult ExecuteEffect(EffectRequest request)
    {
		if (request == null)
		{
			return null;
		}

		EffectResult result = null;

		if (request.RawDamage != 0)
		{
			result = ExecuteDamageEffect(request);
		}
        else if (request.RawHealing != 0)
        {
            result = ExecuteHealingEffect(request);
        }
        else if (request.ModifierTemplate != null)
		{
			result = ExecuteModifierEffect(request);
		}
		else if (request.Trigger != EffectTrigger.None)
		{
			result = ExecuteTriggerEffect(request);
		}

		if (result == null)
		{
            return new EffectResult();
        }
		else
		{
			result.Sender = request.Sender;
			result.Target = request.Target;
			return result;
		}
		
    }

	public void ReceiveEffectResult(EffectResult result)
	{
		foreach (Modifier modifier in _modifiers) 
		{
			modifier.ProcessEffectResult(result);
		}

		ClearModifiersToBeRemovedList();
	}

	public void InterceptEffectRequest(EffectRequest request)
	{
		if (request.Trigger == EffectTrigger.OnRoundEnd)
		{
			if (_timeOnBoard > -1)
				DecrementTimeOnBoard();
		}

		foreach (Modifier modifier in _modifiers)
		{
			modifier.InterceptEffectRequest(request);
		}

		ClearModifiersToBeRemovedList();
	}

	private void DecrementTimeOnBoard()
	{
		_timeOnBoard--;

		if (_timeOnBoard <= 0)
		{
			CallForDeath(null);
		}
	}

	private EffectResult ExecuteDamageEffect(EffectRequest request)
	{
		int hpLoss;
        DamageType damageType = request.DamageType;
		EffectResult effectResult = new();

        switch (damageType)
        {
            case DamageType.Hit:
				hpLoss = StandardDefenseCalculation(request.RawDamage, GetModStat(StatType.Defense));
                break;
			case DamageType.Essence:
                hpLoss = StandardDefenseCalculation(request.RawDamage, GetModStat(StatType.Defense));
                break;
            default:
				return null;
        }

		int effectiveDamage = CalculateEffectiveDamage(hpLoss, this);

		int newHP = GetModStat(StatType.CurHealth) - effectiveDamage;
		SetStat(StatType.CurHealth, newHP);

        if (request.GainDecay)
        {
            int newDecay = GetBaseStat(StatType.CurDecay) + ((effectiveDamage * GetModStat(StatType.DecayRate)) / 100);
            SetStat(StatType.CurDecay, newDecay);
        }

		if (newHP <= 0)
			CallForDeath(request.Sender);

        effectResult.EffectiveDamage = effectiveDamage;
		effectResult.DamageType = damageType;

		return effectResult;
    }

	private EffectResult ExecuteHealingEffect(EffectRequest request)
	{
		EffectResult result = new EffectResult();

		int rawHealing = request.RawHealing;

		if (rawHealing == 0) return null;

		int effectiveHealing = GainHP(rawHealing);
		result.EffectiveHealing = effectiveHealing;

		return result;
	}

	private EffectResult ExecuteModifierEffect(EffectRequest request)
	{
		EffectResult result = new EffectResult();
		
		result.ModifierApplied = ApplyModifier(request.ModifierTemplate, request.ModifierArgs);

		return result;
	}

	private Modifier ApplyModifier(ModifierTemplateWithArgs templateWithArgs, IReadOnlyUnit sender = null)
	{
		return ApplyModifier(templateWithArgs.Template, templateWithArgs.Args, sender);
	}

	private Modifier ApplyModifier(ModifierTemplate template, 
		ReadOnlyDictionary<ModifierFloatArgType, float> args,
		IReadOnlyUnit sender = null)
	{
		if (template == null) return null;

		if (sender == null) sender = (IReadOnlyUnit)GetParent();

        Modifier modifier = template.InstantiateAndApply(
            (IReadOnlyUnit)GetParent(),
            sender,
            args);
        AddModifier(modifier);

		return modifier;
    }

	private EffectResult ExecuteTriggerEffect(EffectRequest request)
	{
        EffectResult result = new EffectResult();

		result.TriggerApplied = request.Trigger;

		return result;
	}

	public ReadOnlyDictionary<StatType, int> StatsList { get { return _statsList.StatsDict; } }
    public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
	public int TimeOnBoard { get { return _timeOnBoard; } }
}

public class SendEffectEventArgs : EventArgs
{
	public EffectRequest EffectRequest;
}

public class UpdateUIEventArgs : EventArgs
{

}

public class UnitDeathCallArgs : EventArgs
{
	public IReadOnlyUnit CallingUnit;
	public IReadOnlyUnit Killer;
}

public class ModifierAddOrRemoveEventArgs
{
	public Modifier Modifier;
}