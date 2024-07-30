using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

public partial class Unit : CanvasItem
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
	private EmotionTag _emotion;
	private int _timeOnBoard = -1;
	
	public Unit(
		StatsList statsList,
        ReadOnlyCollection<Move> moveSet,
        UnitSprite sprite,
		UnitUI uI,
        EmotionTag emotion,
		string friendlyName,
		int timeOnBoard) : base()
	{
		_sprite = sprite;
		AddChild(sprite);

		_stats = new UnitStats(statsList);

        _uI = uI;
		AddChild(_uI);
        _uI?.Update(_stats);

        _moveSet = moveSet;
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
		_stats.UpdateUIEventHandler += UpdateUI;
		_stats.ZeroHPEventHandler += EmitZeroHPSignal;
		_stats.EnqueueEffectInputEventHandler += EnqueueEffectInput;
		_stats.SendEffectEventHandler += SendEffect;
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

    public EffectResult ExecuteEffect(EffectRequest effectResult)
    {
        return _stats.ExecuteEffect(effectResult);
    }

    public int GetModStat(StatType stat)
	{
		return _stats.GetModStat(stat);
	}

	public int GetBaseStat(StatType stat)
	{
		return _stats.GetBaseStat(stat);
	}

	public event EventHandler<ZeroHPEventArgs> ZeroHPEventHandler;

	private void EmitZeroHPSignal(object sender, ZeroHPEventArgs e)
	{
		ZeroHPEventHandler(this, e);
	}

	//
	// UI related functions
	//
	public void UpdateUI(object sender, UpdateUIEventArgs e)
	{
		UnitStats stats = sender as UnitStats;

		if (stats != null)
		{
            _uI.Update(stats);
        }
	}

	public void InitializeUI()
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
		SendEffectEventHandler(this, e);
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
	public UnitStats Stats { get { return _stats; } }
	public ReadOnlyCollection<Move> MoveSet { get { return _moveSet; } }
	public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
	public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
	public UnitUI UI { get { return _uI; } }
	public UnitSprite Sprite { get { return _sprite; } }
	public string FriendlyName { get { return _friendlyName; } }
}
