using System.Collections.ObjectModel;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;
using SoulSmith.Battle;
using SoulSmith.Battle.Move;
using SoulSmith.Core;
using SoulSmith.Templates;
using SoulSmith.Asset;
using SoulSmith.Shapes;
using SoulSmith.Drawing;
using SoulSmith.Battle.Effect;
using SoulSmith.Battle.Effect.Trigger;

namespace SoulSmith.Units;
public class Unit : CanvasObject, IReadOnlyUnit
{
	//Children
	private UnitUI _uI;
	private UnitSprite _sprite;
	private UnitStats _stats;

	private string _friendlyName;
	private ReadOnlyCollection<Move> _moveSet;
	private bool _inCombat = false;
	private bool _playerControlled = false;
	private int _combatPosition;
	private EmotionTag.EmotionTag _emotion;
	private int _timeOnBoard = -1;
	private IZone _hitZone = null;
	private IZone _fireZone = null;

	public Unit(UnitTemplate template) : this(
		new StatsList(template.StatsList),
		Move.GenerateMoveList(template.MoveSetWeightedList, template.MaxMoveCount),
        new UnitSprite(
			DrawHelpers.GetDrawableResource(template.SpriteName, template.SpriteType), 
			Rand.RandDoubleAroundOne(UnitSprite.ANIMATIONDESYNCFACTORRADIUS)), 
		new UnitUI(),
		template.Emotion,
		template.FriendlyName,
		template.TimeOnBoard)
	{ }

	public Unit(
		StatsList statsList,
		IEnumerable<Move> moveSet,
		UnitSprite sprite,
		UnitUI uI,
		EmotionTag.EmotionTag emotion,
		string friendlyName,
		int timeOnBoard) : base()
	{
        _sprite = sprite;
		_sprite.UpdateResourceState(UnitSprite.SPRITEIDLESTATE);
		AddChild(sprite);

        _stats = new UnitStats(statsList, timeOnBoard, emotion);
		AddChild(_stats);

        _uI = uI;
		AddChild(_uI);
        _uI?.Update(_stats);

        _moveSet = new ReadOnlyCollection<Move>(moveSet.ToList());
		_emotion = emotion;
		_friendlyName = friendlyName;
		_timeOnBoard = timeOnBoard;

        Initialize();
    }

    private void Initialize()
	{
		InitializeStats();
        InitializeUI();
    }

	private void InitializeStats()
	{
		_stats.UnitDeathCallEventHandler += EmitUnitDeathCallSignal;
		_stats.EnqueueEffectInputEventHandler += EnqueueEffectInput;
		_stats.SendEffectEventHandler += SendEffect;
        _stats.ModifierAddEventHandler += OnModifierAdded;
        _stats.ModifierRemoveEventHandler += OnModifierRemoved;
        _stats.LoadEmotionAttributes(_emotion);
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
	}

	public void RemoveFromCombat()
	{
		_inCombat = false;
		_combatPosition = -1;
	}

    public EffectResult ExecuteEffect(EffectRequest request)
    {
		EffectResult result = null;

		_stats.InterceptEffectRequest(request);

        if (request.Sender == this)
        {
			if (request.Trigger == EffectTrigger.OnMoveBegin)
				_sprite.UpdateResourceState(UnitSprite.SPRITEATTACKSTATE);
        }

		if (request.Target == this)
		{
			result = _stats.ExecuteEffect(request);
		}

		UpdateUI();
		UpdateSprite();

		return result;
    }

	public void ReceiveEffectResult(EffectResult result)
	{
		_stats.ReceiveEffectResult(result);
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

	private void EmitUnitDeathCallSignal(object sender, UnitDeathCallArgs e)
	{
		UnitDeathCallEventHandler(this, e);

		_sprite.UpdateResourceState(UnitSprite.SPRITEDEATHSTATE);
	}

	private void UpdateSprite()
	{
		if (_sprite != null)
		{
			_sprite.Update(_stats);
		}
	}

	//
	// UI related functions
	//
	private void UpdateUI()
	{
        if (_stats != null)
        {
            _uI.Update(_stats);
        }
    }

	private void InitializeUI()
	{ 
		_uI.MoveButtonPressedEventHandler += OnMoveButtonPressed;
		_uI.TargetButtonPressedEventHandler += OnTargetButtonPressed;

		_uI.Update(_stats);
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
		OfferMoveAndUserEventHandler(this, args);
	}

	public event EventHandler<TargetButtonPressedEventArgs> OfferTargetEventHandler;

	//Listens to UI TargetButtonPressed
	private void OnTargetButtonPressed(object sender, TargetButtonPressedEventArgs args)
	{
		args.Target = this;
		OfferTargetEventHandler(this, args);
	}

    public event EventHandler<SendEffectEventArgs> SendEffectEventHandler;

    //Listens to stats SendEffectRequest
    private void SendEffect(object sender, SendEffectEventArgs e)
    {
		EffectRequest request = e.EffectRequest;

		SendEffectEventHandler(this, e);
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
	public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
	public UnitUI UI { get { return _uI; } }
	public UnitSprite Sprite { get { return _sprite; } }
	public string FriendlyName { get { return _friendlyName; } }
	public IZone HitZone { get { return _hitZone; } }
	public IZone FireZone { get { return _fireZone; } }
    public int MaxHealth { get { return _stats.GetModStat(StatType.MaxHealth); } }
    public int CurHealth { get { return _stats.GetModStat(StatType.CurHealth); } }
    public int Attack { get { return _stats.GetModStat(StatType.Attack); } }
    public int Defense { get { return _stats.GetModStat(StatType.Defense); } }
    public int DecayRate { get { return _stats.GetModStat(StatType.DecayRate); } }
    public int CurDecay { get { return _stats.GetModStat(StatType.CurDecay); } }
}
