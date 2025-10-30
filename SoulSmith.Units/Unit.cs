using System.Collections.ObjectModel;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;
using SoulSmith.Battle;
using SoulSmith.Battle.Moves;
using SoulSmith.Core;
using SoulSmith.Templates;
using SoulSmith.Asset;
using SoulSmith.Shapes;
using SoulSmith.Drawing;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Units;
public class Unit : CanvasObject, IReadOnlyUnit
{
	//Children
	private UnitUI _uI;
	private UnitSprite _sprite;
	private UnitStats _stats;

	private string _spriteKey;
    private string _friendlyName;
	private ReadOnlyCollection<Move> _moveSet;
	private bool _inCombat = false;
	private bool _playerControlled = false;
	private int _combatPosition;
	private EmotionTags.EmotionTag _emotionTag;

	public Unit(UnitTemplate template) : this(
		new StatsList(template.StatsList),
		Move.GenerateMoveList(template.MoveSetWeightedList, template.MaxMoveCount),
        new UnitSprite(
			DrawHelpers.GetDrawableResourceInstance(template.SpriteName), 
			Rand.RandDoubleAroundOne(UnitSprite.ANIMATIONDESYNCFACTORRADIUS)), 
		new UnitUI(),
		template.Emotion,
		template.FriendlyName,
		template.TimeOnBoard,
		template.SpriteName,
		template.SpriteSizeMod)
	{ }

	public Unit(
		StatsList statsList,
		IEnumerable<Move> moveSet,
		UnitSprite sprite,
		UnitUI uI,
		EmotionTags.EmotionTag emotion,
		string friendlyName,
		int timeOnBoard,
		string spriteKey,
		float spriteSizemod) : base()
	{
		_spriteKey = spriteKey;

        _sprite = sprite;
		_sprite.UpdateResourceState(UnitSprite.SPRITEIDLESTATE);
		_sprite.Scale(spriteSizemod);
		AddChild(sprite);

        _stats = new UnitStats(statsList, timeOnBoard, emotion);
		AddChild(_stats);

        _uI = uI;
		AddChild(_uI);
        _uI?.Update((IReadOnlyUnit)this);

        _moveSet = new ReadOnlyCollection<Move>(moveSet.ToList());
		_emotionTag = emotion;
		_friendlyName = friendlyName;

        Initialize();
    }

    private void Initialize()
	{
		InitializeStats();
        InitializeUI();
    }

	private void InitializeStats()
	{
		_stats.UnitDeathCallEventHandler += CallForDeath;
		_stats.EnqueueEffectInputEventHandler += EnqueueEffectInput;
        _stats.ModifierAddEventHandler += OnModifierAdded;
        _stats.ModifierRemoveEventHandler += OnModifierRemoved;
    }

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    private void EnqueueEffectInput(object sender, EnqueueEffectInputEventArgs e)
	{
		EnqueueEffectInputEventHandler(this, e);
	}

	//
	// Combat related functions
	//

	//Everything the unit needs to do to prepare for combat
	public void OnJoinCombat()
	{
		_inCombat = true;
		_stats.CombatPosition = _combatPosition;
		EnqueueEmotionCombatEntryEffects();
    }

	private void EnqueueEmotionCombatEntryEffects()
	{
        Battle.Emotions.Emotion emotion = Battle.Emotions.Emotion.GetEmotion(_emotionTag);

		if (emotion == null) return;
		if (emotion.BattleEntryEffects == null) return;
		if (emotion.BattleEntryEffects.Count == 0) return;

		foreach (IEffect effect in emotion.BattleEntryEffects)
		{
			EnqueueEffectInputEventArgs args = new EnqueueEffectInputEventArgs();
			args.EffectInput = new EffectInput(effect, this, this, Priority.EmotionCombatEntryEffect, null, null);
			EnqueueEffectInput(this, args);
        }
	}

    public void OnRetreat()
	{
		_inCombat = false;
		_combatPosition = -1;
		_stats.OnRetreat();
	}

	public void OnDeath()
	{
		_inCombat = false;
		_combatPosition = -1;
		_stats.OnDeath();
	}

    public void ModifyEffectRequest(PayloadBase request)
    {
		_stats.ModifyPayload(request);
    }

	public ResultBase ExecuteEffectRequest(PayloadBase request)
	{
		ResultBase result = null;

		result = _stats.ExecutePayload(request);

        UpdateUI();

        return result;
	}

	public void ReactToPayloadResult(ResultBase result)
	{
		switch (result)
		{
			case TriggerResult triggerResult:
				if (triggerResult.Trigger == CombatTrigger.OnMoveBegin)
				{
					if (triggerResult.Sender == this) // Start attack animation if this unit is triggering a move begin
						_sprite.PlayAttackAnimation();
				} else if (triggerResult.Trigger == CombatTrigger.OnUnitDeath)
				{
					//if (triggerResult.Target == this)
					//	_sprite.PlayDeathAnimation();
				}	
				break;
			case DamageResult damageResult:
				if (damageResult.Target == this)
					if (damageResult.EffectiveDamage > 0)
						_sprite.PlayHurtAnimation();
				break;
        }

        _stats.ReactToPayloadResult(result);

		UpdateUI();
	}

	public void PlayDeathAnimation()
	{
		_sprite.PlayDeathAnimation();
    }

    public int GetModStat(StatType stat)
	{
		return _stats.GetModStat(stat);
	}

	public int GetBaseStat(StatType stat)
	{
		return _stats.GetBaseStat(stat);
	}

	public event EventHandler<UnitDeathCallArgs> UnitDeathCallEventHandler;

	private void CallForDeath(object sender, UnitDeathCallArgs e)
	{
		UnitDeathCallEventHandler(this, e);

		_sprite.UpdateResourceState(UnitSprite.SPRITEDEATHSTATE);
	}

	public event EventHandler<UnitRetreatCallArgs> UnitRetreatCallEventHandler;

	private void OnRetrieveButtonPressed(object sender, RetrieveButtonPressedEventArgs e)
	{
		UnitRetreatCallArgs args = new UnitRetreatCallArgs();
		args.RetreatingUnit = this;
		args.FromRetrieveButton = true;
        UnitRetreatCallEventHandler?.Invoke(this, args);
	}

	//
	// UI related functions
	//
	private void UpdateUI()
	{
		_uI.Update((IReadOnlyUnit)this);
    }

	private void InitializeUI()
	{ 
		_uI.MoveButtonPressedEventHandler += OnMoveButtonPressed;
		_uI.RetrieveButtonPressedEventHandler += OnRetrieveButtonPressed;
        _uI.TargetButtonPressedEventHandler += OnTargetButtonPressed;

		_uI.Update((IReadOnlyUnit)this);
		_uI.UpdateMoveMenu(_moveSet); 
	}

	public void ShowMoveSelectUI()
	{
		if (_moveSet.Count > 0)
		{
			_uI.ShowMoveSelect();
		}
	}

	public void ShowTargetSelectUI()
	{
		_uI.ShowTargetSelect();
	}

	public event EventHandler<MoveButtonPressedEventArgs> OfferMoveAndUserEventHandler;

	//Listens to UI MoveButtonPressed
	private void OnMoveButtonPressed(object sender, MoveButtonPressedEventArgs args)
	{
		args.Sender = this;
		OfferMoveAndUserEventHandler?.Invoke(this, args);
	}

    public event EventHandler<TargetButtonPressedEventArgs> OfferTargetEventHandler;

	//Listens to UI TargetButtonPressed
	private void OnTargetButtonPressed(object sender, TargetButtonPressedEventArgs args)
	{
		args.Target = this;
		OfferTargetEventHandler?.Invoke(this, args);
	}

	private void OnModifierAdded(object sender, ModifierAddOrRemoveEventArgs e)
	{
		_uI.OnModifierAdded(e.Modifier);
	}

	private void OnModifierRemoved(object sender, ModifierAddOrRemoveEventArgs e)
	{
		_uI.OnModifierRemoved(e.Modifier);
	}

    public void HideMoveSelectUI()
	{
		_uI.HideMoveSelect();
	}

	public void HideTargetSelectUI()
	{
		_uI.HideTargetSelect();
	}

    public bool InCombat { get { return _inCombat; } }
	public IReadOnlyUnitStats ReadOnlyStats { get { return _stats; } }
	public UnitStats Stats { get { return _stats; } }
	public ReadOnlyDictionary<StatType, int> StatsList { get { return _stats.StatsList; } }
	public ReadOnlyCollection<Move> MoveSet { get { return _moveSet; } }
	public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
	public UnitUI UI { get { return _uI; } }
	public UnitSprite Sprite { get { return _sprite; } }
	public IReadOnlyUnitSprite ReadOnlySprite { get { return _sprite; } }
	public EmotionTags.EmotionTag EmotionTag { get { return _emotionTag; } }
    public string FriendlyName { get { return _friendlyName; } }
    public int MaxHealth { get { return _stats.GetModStat(StatType.MaxHealth); } }
    public int CurHealth { get { return _stats.GetModStat(StatType.CurHealth); } }
    public int Attack { get { return _stats.GetModStat(StatType.Attack); } }
    public int Defense { get { return _stats.GetModStat(StatType.Defense); } }
    public int DecayRate { get { return _stats.GetModStat(StatType.DecayRate); } }
    public int CurDecay { get { return _stats.GetModStat(StatType.CurDecay); } }
	public int TimeOnBoard { get { return _stats.TimeOnBoard; } }
	public string SpriteKey { get { return _spriteKey; } }
}

public class UnitRetreatCallArgs : EventArgs
{
	public bool FromRetrieveButton { get; set; }
    public Unit RetreatingUnit { get; set; }
}